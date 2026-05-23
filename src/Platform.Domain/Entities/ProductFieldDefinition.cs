using Platform.Domain.Common;
using Platform.Domain.Enums;

namespace Platform.Domain.Entities;

public sealed class ProductFieldDefinition : Entity, IStoreScoped
{
    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public ProductFieldType FieldType { get; set; } = ProductFieldType.Text;
    public bool IsRequired { get; set; }
    public bool IsVisibleOnListing { get; set; } = true;
    public int DisplayOrder { get; set; }
    public string OptionsJson { get; set; } = "{}";
}
