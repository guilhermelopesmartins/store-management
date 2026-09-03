namespace StoreManagement.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class SkipCompanyValidationAttribute : Attribute
{
}