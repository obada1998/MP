using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public sealed class ProductCustomFieldValue : Entity, IStoreScoped
{
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid ProductFieldDefinitionId { get; set; }
    public ProductFieldDefinition FieldDefinition { get; set; } = null!;
    public string ValueJson { get; set; } = "null";
}
