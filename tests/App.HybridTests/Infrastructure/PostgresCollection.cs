using Xunit;

namespace App.HybridTests.Infrastructure;

[CollectionDefinition("postgres")]
public sealed class PostgresCollection : ICollectionFixture<PostgresContainerFixture>
{
}
