namespace Platform.Blazor.Security;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class ClientAuthorizeAttribute : Attribute
{
    public string? Roles { get; set; }
}
