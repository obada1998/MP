namespace Platform.Domain.Common;

public interface IStoreScoped
{
    Guid StoreId { get; set; }
}
