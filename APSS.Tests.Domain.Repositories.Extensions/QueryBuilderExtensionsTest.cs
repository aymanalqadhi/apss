using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Exceptions;
using APSS.Domain.Repositories.Extensions;
using APSS.Tests.Domain.Entities.Validators;
using APSS.Tests.Infrastructure.Repositories.EntityFramework.Util;
using APSS.Tests.Utils;

using Xunit;

namespace APSS.Tests.Domain.Repositories.Extensions;

public sealed class QueryBuilderExtensionsTest
{
    #region Fields

    private readonly IUnitOfWork _uow;

    #endregion Fields

    #region Public Constructors

    public QueryBuilderExtensionsTest()
        => _uow = TestUnitOfWork.Create();

    #endregion Public Constructors

    #region Public Methods

    [Fact]
    public async Task FindFact()
    {
        var logs = Enumerable
            .Range(0, RandomGenerator.NextInt(10, 20))
            .Select(_ => ValidEntitiesFactory.CreateValidLog())
            .ToArray();

        _uow.Logs.Add(logs);
        Assert.Equal(logs.Length, await _uow.CommitAsync());

        foreach (var log in logs)
        {
            var foundLog1 = await _uow.Logs.Query().FindAsync(log.Id);
            var foundLog2 = await _uow.Logs.Query().Find(log.Id).FirstAsync();

            Assert.Equal(foundLog1.Id, foundLog2.Id);
            Assert.Equal(foundLog1.Message, foundLog2.Message);
            Assert.Equal(foundLog1.Severity, foundLog2.Severity);

            Assert.Equal(log.Id, foundLog1.Id);
            Assert.Equal(log.Message, foundLog1.Message);
            Assert.Equal(log.Severity, foundLog1.Severity);
        }

        var generateNonExistentId = () =>
        {
            while (true)
            {
                var id = RandomGenerator.NextLong(0);

                if (!logs.Any(l => l.Id == id))
                    return id;
            }
        };

        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await _uow.Logs.Query().FindAsync(generateNonExistentId());
        });

        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await _uow.Logs.Query().Find(generateNonExistentId()).FirstAsync();
        });
    }

    [Fact]
    public async Task ContainsFact()
    {
        var logs = Enumerable
            .Range(0, RandomGenerator.NextInt(10, 20))
            .Select(_ => ValidEntitiesFactory.CreateValidLog())
            .ToArray();

        _uow.Logs.Add(logs);
        Assert.Equal(logs.Length, await _uow.CommitAsync());

        foreach (var log in logs)
            Assert.True(await _uow.Logs.Query().ContainsAsync(log));

        var nonExistentLog = ValidEntitiesFactory.CreateValidLog();

        Assert.False(await _uow.Logs.Query().ContainsAsync(nonExistentLog));
    }

    #endregion Public Methods
}