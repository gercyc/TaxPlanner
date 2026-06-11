using Markdig;
using Microsoft.EntityFrameworkCore;
using TaxPlanner.Server.Data;
using TaxPlanner.Server.Models;

namespace TaxPlanner.Server.Services;

public class BlogService
{
    private readonly ApplicationDbContext _db;

    public BlogService(ApplicationDbContext db)
    {
        _db = db;
    }

    // ── Public API ──────────────────────────────────────────────────────────

    public async Task<List<Post>> GetPublishedPostsAsync(int page = 1, int pageSize = 10)
    {
        return await _db.Posts
            .Where(p => p.IsPublished && p.PublishedAt.HasValue)
            .OrderByDescending(p => p.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetPublishedCountAsync()
    {
        return await _db.Posts.CountAsync(p => p.IsPublished && p.PublishedAt.HasValue);
    }

    public async Task<Post?> GetBySlugAsync(string slug)
    {
        return await _db.Posts.FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);
    }

    // ── Admin CRUD ──────────────────────────────────────────────────────────

    public async Task<List<Post>> GetAllPostsAsync()
    {
        return await _db.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _db.Posts.FindAsync(id);
    }

    public async Task<Post> CreateAsync(string title, string? summary, string contentMarkdown, string? tags, string slug)
    {
        var post = new Post
        {
            Title = title.Trim(),
            Summary = summary?.Trim(),
            ContentMarkdown = contentMarkdown,
            ContentHtml = RenderMarkdown(contentMarkdown),
            Tags = tags?.Trim(),
            Slug = string.IsNullOrWhiteSpace(slug) ? GenerateSlug(title) : slug.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsPublished = false
        };

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();
        return post;
    }

    public async Task<Post?> UpdateAsync(int id, string title, string? summary, string contentMarkdown, string? tags, string slug, bool isPublished)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return null;

        post.Title = title.Trim();
        post.Summary = summary?.Trim();
        post.ContentMarkdown = contentMarkdown;
        post.ContentHtml = RenderMarkdown(contentMarkdown);
        post.Tags = tags?.Trim();
        post.Slug = string.IsNullOrWhiteSpace(slug) ? GenerateSlug(title) : slug.Trim();
        post.UpdatedAt = DateTime.UtcNow;
        post.IsPublished = isPublished;

        if (isPublished && post.PublishedAt is null)
            post.PublishedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return post;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return false;

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PublishAsync(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return false;

        post.IsPublished = true;
        post.PublishedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnpublishAsync(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return false;

        post.IsPublished = false;
        post.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    // ── Image management ───────────────────────────────────────────────────

    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"
    };

    private const long MaxImageSize = 5 * 1024 * 1024; // 5 MB

    public record ImageInfo(string FileName, string Url, long Size, DateTime CreatedAt);

    public async Task<string> UploadImageAsync(Stream stream, string fileName, string contentType)
    {
        var ext = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(ext) || !AllowedImageExtensions.Contains(ext))
            throw new InvalidOperationException(
                $"Tipo de ficheiro inválido. Formatos permitidos: {string.Join(", ", AllowedImageExtensions)}");

        if (stream.Length > MaxImageSize)
            throw new InvalidOperationException($"Ficheiro excede o limite de {MaxImageSize / 1024 / 1024} MB");

        var safeName = $"{Guid.NewGuid():N}{ext}";
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posts");
        Directory.CreateDirectory(uploadsDir);

        var filePath = Path.Combine(uploadsDir, safeName);
        await using var fileStream = File.Create(filePath);
        await stream.CopyToAsync(fileStream);

        return $"/uploads/posts/{safeName}";
    }

    public List<ImageInfo> GetImages()
    {
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posts");
        if (!Directory.Exists(uploadsDir))
            return new List<ImageInfo>();

        return Directory.GetFiles(uploadsDir)
            .Select(f => new FileInfo(f))
            .Where(fi => AllowedImageExtensions.Contains(fi.Extension))
            .Select(fi => new ImageInfo(
                fi.Name,
                $"/uploads/posts/{fi.Name}",
                fi.Length,
                fi.CreationTime
            ))
            .OrderByDescending(i => i.CreatedAt)
            .ToList();
    }

    public bool DeleteImage(string fileName)
    {
        if (fileName.Contains("..") || fileName.Contains('/') || fileName.Contains('\\'))
            return false;

        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posts");
        var filePath = Path.Combine(uploadsDir, fileName);

        if (!File.Exists(filePath))
            return false;

        File.Delete(filePath);
        return true;
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static string RenderMarkdown(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
        return Markdown.ToHtml(markdown, pipeline);
    }

    private static string GenerateSlug(string title)
    {
        var slug = title
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("ç", "c").Replace("ã", "a").Replace("á", "a")
            .Replace("à", "a").Replace("â", "a").Replace("é", "e")
            .Replace("ê", "e").Replace("í", "i").Replace("ó", "o")
            .Replace("ô", "o").Replace("õ", "o").Replace("ú", "u")
            .Replace("ü", "u").Replace("ñ", "n");

        // Remove any non-alphanumeric, non-hyphen characters
        slug = string.Join("", slug.Select(c => (char.IsLetterOrDigit(c) || c == '-') ? c : '-').ToArray());
        // Collapse multiple hyphens
        while (slug.Contains("--"))
            slug = slug.Replace("--", "-");
        return slug.Trim('-');
    }
}
