using Microsoft.AspNetCore.Mvc;
using TaxPlanner.Server.Services;

namespace TaxPlanner.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly BlogService _blog;

    public PostsController(BlogService blog)
    {
        _blog = blog;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var posts = await _blog.GetPublishedPostsAsync(page, pageSize);
        var total = await _blog.GetPublishedCountAsync();

        return Ok(new
        {
            posts = posts.Select(p => new
            {
                p.Slug,
                p.Title,
                p.Summary,
                p.Tags,
                p.PublishedAt,
                p.UpdatedAt
            }),
            page,
            pageSize,
            totalCount = total
        });
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Get(string slug)
    {
        var post = await _blog.GetBySlugAsync(slug);
        if (post is null) return NotFound(new { error = "Post not found" });

        return Ok(new
        {
            post.Slug,
            post.Title,
            post.Summary,
            post.ContentHtml,
            post.ContentMarkdown,
            post.Tags,
            post.PublishedAt,
            post.UpdatedAt
        });
    }

    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"
    };

    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    [HttpPost("upload-image")]
    [IgnoreAntiforgeryToken]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "No file provided" });

        if (file.Length > MaxFileSize)
            return BadRequest(new { error = $"File exceeds {MaxFileSize / 1024 / 1024} MB limit" });

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(ext) || !AllowedImageExtensions.Contains(ext))
            return BadRequest(new { error = $"Invalid file type. Allowed: {string.Join(", ", AllowedImageExtensions)}" });

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posts");
        Directory.CreateDirectory(uploadsDir);

        var filePath = Path.Combine(uploadsDir, fileName);
        await using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream);

        var url = $"/uploads/posts/{fileName}";
        return Ok(new { url, fileName });
    }

    [HttpGet("rss")]
    [Produces("application/rss+xml")]
    public async Task<IActionResult> Rss()
    {
        var posts = await _blog.GetPublishedPostsAsync(1, 30);
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var itemsXml = new System.Text.StringBuilder();
        foreach (var post in posts)
        {
            var link = $"{baseUrl}/post/{post.Slug}";
            var pubDate = (post.PublishedAt ?? post.CreatedAt).ToString("R");
            var description = System.Net.WebUtility.HtmlEncode(post.Summary ?? "");

            itemsXml.AppendLine("    <item>");
            itemsXml.AppendLine($"      <title>{System.Net.WebUtility.HtmlEncode(post.Title)}</title>");
            itemsXml.AppendLine($"      <link>{link}</link>");
            itemsXml.AppendLine($"      <guid isPermaLink=\"true\">{link}</guid>");
            itemsXml.AppendLine($"      <pubDate>{pubDate}</pubDate>");
            itemsXml.AppendLine($"      <description>{description}</description>");
            if (!string.IsNullOrWhiteSpace(post.Tags))
            {
                foreach (var tag in post.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    itemsXml.AppendLine($"      <category>{System.Net.WebUtility.HtmlEncode(tag)}</category>");
                }
            }
            itemsXml.AppendLine("    </item>");
        }

        var rss = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0"" xmlns:atom=""http://www.w3.org/2005/Atom"">
  <channel>
    <title>TaxPlanner Blog</title>
    <link>{baseUrl}/blog</link>
    <description>Artigos sobre planejamento tributário, legislação fiscal e gestão de impostos</description>
    <language>pt-BR</language>
    <lastBuildDate>{DateTime.UtcNow.ToString("R")}</lastBuildDate>
    <atom:link href=""{baseUrl}/api/posts/rss"" rel=""self"" type=""application/rss+xml""/>
{itemsXml}  </channel>
</rss>";

        return Content(rss, "application/rss+xml", System.Text.Encoding.UTF8);
    }
}
