using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Tests.Utils;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APSS.Tests.Domain.Entities.Validators;

[TestClass]
public class AccountValidatorTests
{
    #region Private fields

    private readonly AccountValidator _validator = new();

    #endregion Private fields

    #region Tests

    [TestMethod]
    public void ValidateAccountShouldSucceed()
    {
        var account = new Account
        {
            HolderName = RandomGenerator.NextString(0xff),
            NationalId = RandomGenerator.NextString(0xff),
        };

        Assert.IsTrue(_validator.Validate(account).IsValid);
    }

    [TestMethod]
    public void ValidateAccountShouldFail()
    {
        var account = new Account
        {
            HolderName = "",
            NationalId = "",
        };

        var result = _validator.TestValidate(account);

        result.ShouldHaveValidationErrorFor(l => l.HolderName);
        result.ShouldHaveValidationErrorFor(l => l.NationalId);
    }

    #endregion Tests
}