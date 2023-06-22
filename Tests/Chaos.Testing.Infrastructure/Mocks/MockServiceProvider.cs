using Moq;

namespace Chaos.Testing.Infrastructure.Mocks;

public class MockServiceProvider
{
    public static Mock<IServiceProvider> Create(Action<Mock<IServiceProvider>>? setup = null)
    {
        var mock = new Mock<IServiceProvider>();
        setup?.Invoke(mock);

        return mock;
    }

    public static MockServiceProviderBuilder CreateBuilder() => new(new Mock<IServiceProvider>());

    public sealed class MockServiceProviderBuilder
    {
        private readonly Mock<IServiceProvider> Mock;

        internal MockServiceProviderBuilder(Mock<IServiceProvider> mock) => Mock = mock;

        public Mock<IServiceProvider> Build() => Mock;

        public MockServiceProviderBuilder SetupService<T>(T service) where T: class
        {
            Mock.Setup(sp => sp.GetService(typeof(T))).Returns(service);

            return this;
        }
    }
}