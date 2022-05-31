using APSS.Infrastructure.Repositores.EntityFramework;
using APSS.Tests.Domain.Entities.Validators;
using APSS.Tests.Infrastructure.Repositories.EntityFramework.Util;
using APSS.Tests.Utils;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

namespace APSS.Tests.Infrastructure.Repositories.EntityFramework;

[TestClass]
public class RepositoryTests
{
    [TestMethod]
    public void AddShouldSucceed()
    {
        using var ctx = TestDbContext.Create();
        using var uow = new ApssUnitOfWork(ctx);

        var log = ValidEntitiesFactory.CreateValidLog();

        uow.Logs.Add(log);

        Assert.AreEqual(uow.Commit(), 1);
        Assert.IsTrue(ctx.Logs.Any(l => l.Id == log.Id));
    }

    [TestMethod]
    public void UpdateShouldSucceed()
    {
        using var ctx = TestDbContext.Create();
        using var uow = new ApssUnitOfWork(ctx);

        var log = ValidEntitiesFactory.CreateValidLog();

        uow.Logs.Add(log);

        Assert.AreEqual(uow.Commit(), 1);
        Assert.IsTrue(ctx.Logs.Any(l => l.Id == log.Id));

        log.Message = RandomGenerator.NextString(0xff);

        uow.Logs.Update(log);

        Assert.AreEqual(uow.Commit(), 1);
        Assert.IsTrue(ctx.Logs.Any(l => l.Id == log.Id && l.Message == log.Message));
    }

    [TestMethod]
    public void DeleteShouldSucceed()
    {
        using var ctx = TestDbContext.Create();
        using var uow = new ApssUnitOfWork(ctx);

        var log = ValidEntitiesFactory.CreateValidLog();

        uow.Logs.Add(log);

        Assert.AreEqual(uow.Commit(), 1);
        Assert.IsTrue(ctx.Logs.Any(l => l.Id == log.Id));

        uow.Logs.Remove(log);

        Assert.AreEqual(uow.Commit(), 1);
        Assert.IsFalse(ctx.Logs.Any(l => l.Id == log.Id));
    }
}