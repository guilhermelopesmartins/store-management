using FluentAssertions;
using StoreManagement.Domain.Exceptions;
using Xunit;

namespace StoreManagement.UnitTests.Domain;

public class DomainExceptionsTests
{
    [Fact]
    public void StoreNotFoundException_ShouldBeADomainException()
    {
        var storeId = Guid.NewGuid();

        var exception = new StoreNotFoundException(storeId);

        exception.Should().BeAssignableTo<DomainException>();
        exception.StoreId.Should().Be(storeId);
        exception.Message.Should().Contain(storeId.ToString());
    }

    [Fact]
    public void StoreAccessDeniedException_ShouldBeADomainException()
    {
        var storeId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var exception = new StoreAccessDeniedException(storeId, companyId);

        exception.Should().BeAssignableTo<DomainException>();
        exception.StoreId.Should().Be(storeId);
        exception.CompanyId.Should().Be(companyId);
        exception.Message.Should().Contain(storeId.ToString());
        exception.Message.Should().Contain(companyId.ToString());
    }

    [Fact]
    public void DomainException_ShouldBeAnException()
    {
        typeof(DomainException).Should().BeAssignableTo<Exception>();
        typeof(DomainException).IsAbstract.Should().BeTrue();
    }
}
