using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Tests.Utils;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APSS.Tests.Domain.Entities.Validators;

[TestClass]
public class UserValidatorTests
{
    #region Private fields

    private readonly UserValidator _validator = new();

    #endregion Private fields

    #region Tests

    [TestMethod]
    public void ValidateUserShouldSucceed()
    {
        var user = new User
        {
            Name = RandomGenerator.NextString(0xff),
            AccessLevel = AccessLevel.Farmer,
            SupervisedBy = new User { AccessLevel = AccessLevel.Group },
        };

        Assert.IsTrue(_validator.Validate(user).IsValid);
    }

    [TestMethod]
    public void ValidateUserShouldFail()
    {
        var user = new User
        {
            Name = "",
            AccessLevel = AccessLevel.Root,
            SupervisedBy = new User { AccessLevel = AccessLevel.Presedint },
        };

        var result = _validator.TestValidate(user);

        result.ShouldHaveValidationErrorFor(l => l.Name);
        result.ShouldHaveValidationErrorFor(l => l.AccessLevel);
        result.ShouldHaveValidationErrorFor(l => l.SupervisedBy);
    }

    #endregion Tests
}