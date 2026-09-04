namespace StoreManagement.Domain.Exceptions;

public sealed class StoreAccessDeniedException : DomainException
{
    public Guid StoreId { get; }
    public Guid CompanyId { get; }

    public StoreAccessDeniedException(Guid storeId, Guid companyId)
        : base($"Company '{companyId}' is not allowed to access store '{storeId}'.")
    {
        StoreId = storeId;
        CompanyId = companyId;
    }
}
