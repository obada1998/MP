namespace Platform.Application.Stores;

public interface IStoreService
{
    Task<IReadOnlyCollection<StoreDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<StoreDto>> GetStoresForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<StoreDto> CreateAsync(string ownerUserId, CreateStoreRequest request, CancellationToken cancellationToken = default);
    Task<StoreDto> GetAsync(string userId, Guid storeId, CancellationToken cancellationToken = default);
    Task<StoreDto> UpdateAsync(string userId, Guid storeId, UpdateStoreRequest request, CancellationToken cancellationToken = default);
    Task<StoreSettingsDto> GetSettingsAsync(string userId, Guid storeId, CancellationToken cancellationToken = default);
    Task<StoreSettingsDto> UpdateSettingsAsync(string userId, Guid storeId, StoreSettingsDto request, CancellationToken cancellationToken = default);
    Task<ThemeSettingsDto> GetThemeAsync(string userId, Guid storeId, CancellationToken cancellationToken = default);
    Task<ThemeSettingsDto> UpdateThemeAsync(string userId, Guid storeId, ThemeSettingsDto request, CancellationToken cancellationToken = default);
}
