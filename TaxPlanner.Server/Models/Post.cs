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

    /// <summary>
    /// URL da imagem de capa (hero/thumbnail) do post. Pode apontar para
    /// um upload local em <c>wwwroot/uploads/posts/</c> ou para uma URL
    /// pública <c>http(s)://...</c>. Quando <c>null</c>, o front público
    /// exibe o placeholder editorial.
    /// </summary>
    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Texto alternativo da imagem de capa, usado por leitores de tela.
    /// Opcional; quando ausente, o front público usa
    /// <c>"Imagem de capa: {title}"</c> como fallback.
    /// </summary>
    [MaxLength(200)]
    public string? ThumbnailAlt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public bool IsPublished { get; set; }
}
