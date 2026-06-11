using System.ComponentModel.DataAnnotations;

namespace TaxPlanner.Server.Models;

public class Post
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Summary { get; set; }

    [Required]
    public string ContentMarkdown { get; set; } = string.Empty;

    [Required]
    public string ContentHtml { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated tags: "dotnet,blazor,aspnetcore"
    /// </summary>
    [MaxLength(500)]
    public string? Tags { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public bool IsPublished { get; set; }
}
