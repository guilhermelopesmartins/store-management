using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using StoreManagement.Api.ExceptionHandling;
using StoreManagement.Domain.Exceptions;
using Xunit;

namespace StoreManagement.UnitTests.ExceptionHandling;

public class GlobalExceptionHandlerTests
{
    private readonly Mock<IProblemDetailsService> _problemDetailsServiceMock;
    private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
    private readonly GlobalExceptionHandler _sut;

    public GlobalExceptionHandlerTests()
    {
        _problemDetailsServiceMock = new Mock<IProblemDetailsService>();
        _problemDetailsServiceMock
            .Setup(s => s.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);

        _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();

        _sut = new GlobalExceptionHandler(_problemDetailsServiceMock.Object, _loggerMock.Object);
    }

    [Theory]
    [InlineData(typeof(StoreNotFoundException), StatusCodes.Status404NotFound)]
    [InlineData(typeof(StoreAccessDeniedException), StatusCodes.Status403Forbidden)]
    public async Task TryHandleAsync_ShouldMapDomainExceptions_AndLogAsWarning(Type exceptionType, int expectedStatusCode)
    {
        // Arrange
        var exception = exceptionType == typeof(StoreNotFoundException)
            ? new StoreNotFoundException(Guid.NewGuid())
            : (Exception)new StoreAccessDeniedException(Guid.NewGuid(), Guid.NewGuid());

        var httpContext = new DefaultHttpContext();

        // Act
        var handled = await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        handled.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(expectedStatusCode);

        _problemDetailsServiceMock.Verify(
            s => s.TryWriteAsync(It.Is<ProblemDetailsContext>(c =>
                c.ProblemDetails.Status == expectedStatusCode &&
                c.Exception == exception)),
            Times.Once);

        VerifyLog(LogLevel.Warning);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldMapFluentValidationException_To400()
    {
        // Arrange
        var exception = new FluentValidation.ValidationException("invalid data");
        var httpContext = new DefaultHttpContext();

        // Act
        var handled = await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        handled.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        _problemDetailsServiceMock.Verify(
            s => s.TryWriteAsync(It.Is<ProblemDetailsContext>(c =>
                c.ProblemDetails.Status == StatusCodes.Status400BadRequest)),
            Times.Once);

        VerifyLog(LogLevel.Warning);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldMapUnknownException_To500_WithoutLeakingDetails()
    {
        // Arrange
        var exception = new InvalidOperationException("some internal secret detail");
        var httpContext = new DefaultHttpContext();

        // Act
        var handled = await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        handled.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        _problemDetailsServiceMock.Verify(
            s => s.TryWriteAsync(It.Is<ProblemDetailsContext>(c =>
                c.ProblemDetails.Status == StatusCodes.Status500InternalServerError &&
                c.ProblemDetails.Detail != null &&
                !c.ProblemDetails.Detail.Contains("some internal secret detail"))),
            Times.Once);

        VerifyLog(LogLevel.Error);
    }

    private void VerifyLog(LogLevel level)
    {
        _loggerMock.Verify(
            l => l.Log(
                level,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
