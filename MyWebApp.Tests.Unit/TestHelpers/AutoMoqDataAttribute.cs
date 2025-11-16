using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace MyWebApp.Tests.Unit.TestHelpers;

/// <summary>
/// Provides auto-generated test data using AutoFixture with Moq customisation.
/// </summary>
/// <remarks>
/// This attribute combines AutoFixture's test data generation with Moq's mocking capabilities.
/// Use [Theory, AutoMoqData] on test methods to automatically generate test data and mocked dependencies.
/// Use [Frozen] attribute on parameters to share the same mock across multiple parameters.
/// </remarks>
public class AutoMoqDataAttribute : AutoDataAttribute
{
    /// <summary>
    /// Initialises a new instance of the <see cref="AutoMoqDataAttribute"/> class.
    /// </summary>
    public AutoMoqDataAttribute()
        : base(() => new Fixture().Customize(new AutoMoqCustomization()))
    {
    }
}
