namespace Platform.Domain.Enums;

public enum StoreMembershipRole
{
    StoreOwner = 1,
    StoreStaff = 2
}

public enum ProductStatus
{
    Draft = 1,
    Active = 2,
    Archived = 3
}

public enum ProductFieldType
{
    Text = 1,
    Number = 2,
    Money = 3,
    Currency = 16,
    TextArea = 4,
    Boolean = 5,
    Toggle = 17,
    Date = 6,
    Time = 18,
    Select = 7,
    MultiSelect = 8,
    Checkbox = 9,
    Radio = 10,
    Image = 11,
    File = 12,
    RichText = 13,
    Color = 14,
    Url = 19,
    Email = 20,
    Phone = 21,
    Json = 15
}

public enum PageLayoutStatus
{
    Draft = 1,
    Published = 2,
    Archived = 3
}

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Paid = 3,
    Fulfilled = 4,
    Cancelled = 5
}
