using System.Net.Http.Json;

namespace TaxPlanner.Wasm.Services;

public class BlogService
{
    private readonly HttpClient _http;

    public BlogService(HttpClient http)
    {
        _http = http;
    }

    public record PostSummary(string Slug, string Title, string? Summary, string? Tags, string? ThumbnailUrl, string? ThumbnailAlt, DateTime? PublishedAt, DateTime? UpdatedAt);
    public record PostDetail(string Slug, string Title, string? Summary, string ContentHtml, string ContentMarkdown, string? Tags, string? ThumbnailUrl, string? ThumbnailAlt, DateTime? PublishedAt, DateTime? UpdatedAt);
    public record PostListResponse(List<PostSummary> Posts, int Page, int PageSize, int TotalCount);

    public async Task<PostListResponse?> GetPostsAsync(int page = 1, int pageSize = 10)
    {
        return await _http.GetFromJsonAsync<PostListResponse>($"/api/posts?page={page}&pageSize={pageSize}");
    }

    public async Task<PostDetail?> GetPostAsync(string slug)
    {
        return await _http.GetFromJsonAsync<PostDetail>($"/api/posts/{slug}");
    }

    public async Task<string?> UploadImageAsync(Stream fileStream, string fileName, string contentType)
    {
        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        content.Add(streamContent, "file", fileName);

        var response = await _http.PostAsync("/api/posts/upload-image", content);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ImageUploadResponse>();
        return result?.Url;
    }

    private record ImageUploadResponse(string Url, string FileName);
}
