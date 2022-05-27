using APSS.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APSS.Domain.Entities.Validators.Test;

[TestClass]
public class LogValidatorTests
{
    #region Private fields
    
    private readonly LogValidator _validator = new();

    #endregion

    #region Tests

    [TestMethod]
    public void ValidateMessageShouldSucceed()
    {
        var log = new Log
        {
            Message = RandomGenerator.NextString(0xff)
        };

        Assert.IsTrue(_validator.Validate(log).IsValid);
    }

    [TestMethod]
    public void ValidateMessageShouldFail()
    {
        var log = new Log { Message = "" };

        Assert.IsFalse(_validator.Validate(log).IsValid);
    }

    #endregion
}
