using System;
using System.Linq;
using System.Threading.Tasks;
using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;
using APSS.Tests.Utils;

using Xunit;

namespace APSS.Tests.Application.App;

public sealed class DatabaseLogsServiceTest
{
    #region Private fields

    private readonly IUnitOfWork _uow;
    private readonly ILogsService _logsSvc;

    #endregion Private fields

    #region Constructors

    public DatabaseLogsServiceTest(IUnitOfWork uow, ILogsService logsSvc)
    {
        _uow = uow;
        _logsSvc = logsSvc;
    }

    #endregion Constructors

    #region Tests

    [Theory]
    [InlineData(LogSeverity.Debug)]
    [InlineData(LogSeverity.Information)]
    [InlineData(LogSeverity.Warning)]
    [InlineData(LogSeverity.Error)]
    [InlineData(LogSeverity.Fatal)]
    public async Task LogAddedTheory(LogSeverity severity)
    {
        var message = RandomGenerator.NextString(0xff);
        var log = await _logsSvc.LogAsync(severity, message);

        Assert.Equal(severity, log.Severity);
        Assert.Equal(message, log.Message);
        Assert.True(await _uow.Logs.Query().ContainsAsync(log));
    }

    [Fact]
    public async Task LogTagsAddedFact()
    {
        var message = RandomGenerator.NextString(0xff);
        var tags = Enumerable
            .Range(0, RandomGenerator.NextInt(5, 20))
            .Select(_ => RandomGenerator.NextString(RandomGenerator.NextInt(5, 20), RandomStringOptions.Alpha))
            .ToArray();

        var log = await _logsSvc.LogDebugAsync(message, tags);

        Assert.All(tags, async tag =>
        {
            Assert.True(await _uow.LogTags.Query().AnyAsync(t => t.Value == tag));
        });

        Assert.All(tags, tag =>
        {
            Assert.Contains(log.Tags, t => t.Value == tag);
        });
    }

    #endregion Tests
}