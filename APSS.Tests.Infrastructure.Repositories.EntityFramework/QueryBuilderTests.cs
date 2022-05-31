using APSS.Tests.Domain.Entities.Validators;
using APSS.Tests.Infrastructure.Repositories.EntityFramework.Util;
using APSS.Tests.Utils;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

namespace APSS.Tests.Infrastructure.Repositories.EntityFramework;

[TestClass]
public class QueryBuilderTests
{
    [TestMethod]
    public void FirstShouldSucceed()
    {
        using var uow = TestUnitOfWork.Create();

        var logs = new[]
        {
            ValidEntitiesFactory.CreateValidLog(),
            ValidEntitiesFactory.CreateValidLog(),
            ValidEntitiesFactory.CreateValidLog(),
        };

        uow.Logs.Add(logs);

        Assert.AreEqual(3, uow.Commit());
        Assert.AreEqual(uow.Logs.Query().FirstAsync().Result.Id, logs[0].Id);

        foreach (var log in logs)
        {
            Assert.IsTrue(
               uow
                   .Logs
                   .Query()
                   .FirstAsync(l => l.Id == log.Id).Result.Message == log.Message
            );
        }
    }

    [TestMethod]
    public void FirstShouldFail()
    {
        using var uow = TestUnitOfWork.Create();

        var exception = Assert.ThrowsException<AggregateException>(() =>
        {
            uow.Logs.Query().FirstAsync().Wait();
        });
        Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));

        var log = ValidEntitiesFactory.CreateValidLog();

        uow.Logs.Add(log);
        Assert.AreEqual(1, uow.Commit());

        exception = Assert.ThrowsException<AggregateException>(() =>
        {
            uow.Logs.Query().FirstAsync(l => l.Id == log.Id + 1).Wait();
        });

        Assert.AreEqual(uow.Logs.Query().FirstAsync(l => l.Id == log.Id).Result.Id, log.Id);
        Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));
    }

    [TestMethod]
    public void CountShouldSucceed()
    {
        using var uow = TestUnitOfWork.Create();

        var logs = Enumerable
            .Range(0, RandomGenerator.NextInt(0, 100))
            .Select(i => ValidEntitiesFactory.CreateValidLog())
            .ToArray();

        uow.Logs.Add(logs);

        Assert.AreEqual(logs.Length, uow.Commit());
        Assert.AreEqual(logs.Length, uow.Logs.Query().CountAsync().Result);
        Assert.AreEqual(
            logs.Count(l => l.Message.Length % 2 == 0),
            uow.Logs.Query().CountAsync(l => l.Message.Length % 2 == 0).Result
        );
    }

    [TestMethod]
    public void HasItemsShouldSucceed()
    {
        using var uow = TestUnitOfWork.Create();

        Assert.IsFalse(uow.Logs.Query().HasItemsAsync().Result);

        var log = ValidEntitiesFactory.CreateValidLog();
        uow.Logs.Add(log);

        Assert.AreEqual(1, uow.Commit());
        Assert.IsTrue(uow.Logs.Query().HasItemsAsync().Result);
    }

    [TestMethod]
    public void AnyShouldSucceed()
    {
        using var uow = TestUnitOfWork.Create();

        var log = ValidEntitiesFactory.CreateValidLog();

        Assert.IsFalse(uow.Logs.Query().AnyAsync(l => l.Message == log.Message).Result);

        uow.Logs.Add(log);

        Assert.AreEqual(1, uow.Commit());
        Assert.IsTrue(uow.Logs.Query().AnyAsync(l => l.Message == log.Message).Result);
    }

    [TestMethod]
    public void AllShouldSucceed()
    {
        using var uow = TestUnitOfWork.Create();

        var logs = Enumerable
            .Range(0, RandomGenerator.NextInt(0, 200))
            .Select(i => ValidEntitiesFactory.CreateValidLog())
            .Where(l => l.Message.Length % 2 == 0)
            .ToArray();

        uow.Logs.Add(logs);

        Assert.AreEqual(logs.Length, uow.Commit());
        Assert.IsTrue(uow.Logs.Query().AllAsync(l => l.Message.Length % 2 == 0).Result);
    }
}