using Microsoft.AspNetCore.Mvc;
using TaxPlanner.Server.Services;

namespace TaxPlanner.Server.Controllers;

[ApiController]
[Route("api/admin/posts")]
public class AdminPostsController : ControllerBase
{
    private readonly BlogService _blog;

    public AdminPostsController(BlogService blog)
    {
        _blog = blog;
    }

    // ── List all posts (admin) ─────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var posts = await _blog.GetAllPostsAsync();
        return Ok(posts.Select(p => new
        {
            p.Id,
            p.Slug,
            p.Title,
            p.Summary,
            p.Tags,
            p.IsPublished,
            p.PublishedAt,
            p.CreatedAt,
            p.UpdatedAt
        }));
    }

    // ── Get by ID ──────────────────────────────────────────────────────

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var post = await _blog.GetByIdAsync(id);
        if (post is null) return NotFound(new { error = "Post not found" });

        return Ok(new
        {
            post.Id,
            post.Slug,
            post.Title,
            post.Summary,
            post.ContentMarkdown,
            post.ContentHtml,
            post.Tags,
            post.IsPublished,
            post.PublishedAt,
            post.CreatedAt,
            post.UpdatedAt
        });
    }

    // ── Create ─────────────────────────────────────────────────────────

    public record CreatePostRequest(
        string Title,
        string? Summary,
        string ContentMarkdown,
        string? Tags,
        string? Slug
    );

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title))
            return BadRequest(new { error = "Title is required" });
        if (string.IsNullOrWhiteSpace(req.ContentMarkdown))
            return BadRequest(new { error = "ContentMarkdown is required" });

        var post = await _blog.CreateAsync(req.Title, req.Summary, req.ContentMarkdown, req.Tags, req.Slug ?? "");

        return CreatedAtAction(nameof(Get), new { id = post.Id }, new
        {
            post.Id,
            post.Slug,
            post.Title,
            post.Summary,
            post.ContentMarkdown,
            post.ContentHtml,
            post.Tags,
            post.IsPublished,
            post.PublishedAt,
            post.CreatedAt,
            post.UpdatedAt
        });
    }

    // ── Update ─────────────────────────────────────────────────────────

    public record UpdatePostRequest(
        string Title,
        string? Summary,
        string ContentMarkdown,
        string? Tags,
        string? Slug,
        bool IsPublished
    );

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title))
            return BadRequest(new { error = "Title is required" });
        if (string.IsNullOrWhiteSpace(req.ContentMarkdown))
            return BadRequest(new { error = "ContentMarkdown is required" });

        var post = await _blog.UpdateAsync(id, req.Title, req.Summary, req.ContentMarkdown, req.Tags, req.Slug ?? "", req.IsPublished);
        if (post is null) return NotFound(new { error = "Post not found" });

        return Ok(new
        {
            post.Id,
            post.Slug,
            post.Title,
            post.Summary,
            post.ContentMarkdown,
            post.ContentHtml,
            post.Tags,
            post.IsPublished,
            post.PublishedAt,
            post.CreatedAt,
            post.UpdatedAt
        });
    }

    // ── Delete ─────────────────────────────────────────────────────────

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _blog.DeleteAsync(id);
        if (!deleted) return NotFound(new { error = "Post not found" });
        return NoContent();
    }

    // ── Publish / Unpublish ────────────────────────────────────────────

    [HttpPut("{id:int}/publish")]
    public async Task<IActionResult> Publish(int id)
    {
        var ok = await _blog.PublishAsync(id);
        if (!ok) return NotFound(new { error = "Post not found" });
        return NoContent();
    }

    [HttpPut("{id:int}/unpublish")]
    public async Task<IActionResult> Unpublish(int id)
    {
        var ok = await _blog.UnpublishAsync(id);
        if (!ok) return NotFound(new { error = "Post not found" });
        return NoContent();
    }

    // ── Image management ───────────────────────────────────────────────

    [HttpGet("images")]
    public IActionResult ListImages()
    {
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posts");
        if (!Directory.Exists(uploadsDir))
            return Ok(new List<object>());

        var files = Directory.GetFiles(uploadsDir)
            .Select(Path.GetFileName)
            .Where(f => f is not null)
            .Select(f => new
            {
                fileName = f,
                url = $"/uploads/posts/{f}",
                size = new FileInfo(Path.Combine(uploadsDir, f!)).Length
            })
            .OrderByDescending(f => new FileInfo(Path.Combine(uploadsDir, f.fileName!)).CreationTime)
            .ToList();

        return Ok(files);
    }

    [HttpDelete("images/{fileName}")]
    [IgnoreAntiforgeryToken]
    public IActionResult DeleteImage(string fileName)
    {
        // Prevent directory traversal
        if (fileName.Contains("..") || fileName.Contains('/') || fileName.Contains('\\'))
            return BadRequest(new { error = "Invalid file name" });

        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posts");
        var filePath = Path.Combine(uploadsDir, fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound(new { error = "Image not found" });

        System.IO.File.Delete(filePath);
        return NoContent();
    }
}
