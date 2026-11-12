using Microsoft.Extensions.Time.Testing;

namespace OpsRelay.UnitTests.Testing;

public static class FakeTimeProviderFactory
{
    public static FakeTimeProvider Create(DateTimeOffset? utcNow = null) =>
        utcNow.HasValue
            ? new FakeTimeProvider(utcNow.Value)
            : new FakeTimeProvider();
}
