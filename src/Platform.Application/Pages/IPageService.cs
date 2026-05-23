namespace Platform.Application.Pages;

public interface IPageService
{
    Task<IReadOnlyCollection<PageDto>> GetPagesAsync(string userId, Guid storeId, CancellationToken cancellationToken = default);
    Task<PageDto> CreatePageAsync(string userId, Guid storeId, UpsertPageRequest request, CancellationToken cancellationToken = default);
    Task<PageDto> UpdatePageAsync(string userId, Guid storeId, Guid pageId, UpsertPageRequest request, CancellationToken cancellationToken = default);
    Task DeletePageAsync(string userId, Guid storeId, Guid pageId, CancellationToken cancellationToken = default);
    Task<PageLayoutDto?> GetLatestLayoutAsync(string userId, Guid storeId, Guid pageId, CancellationToken cancellationToken = default);
    Task<PageLayoutDto> SaveLayoutAsync(string userId, Guid storeId, Guid pageId, SavePageLayoutRequest request, CancellationToken cancellationToken = default);
    Task<PageLayoutDto?> GetPublishedLayoutAsync(Guid storeId, string slug, CancellationToken cancellationToken = default);
}
