using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Tests.Utils;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APSS.Tests.Domain.Entities.Validators;

[TestClass]
public class VoluntaryValidatorTests
{
    #region Private fields

    private readonly VoluntaryValidator _validator = new();

    #endregion Private fields

    #region Tests

    [TestMethod]
    public void ValidateVoulntaryShouldSucceed()
    {
        var voluntary = new Voluntary
        {
            Name = RandomGenerator.NextString(0xff),
            Field = RandomGenerator.NextString(0xff),
        };

        Assert.IsTrue(_validator.Validate(voluntary).IsValid);
    }

    [TestMethod]
    public void ValidateVoluntaryShouldFail()
    {
        var voluntary = new Voluntary
        {
            Name = "",
            Field = "",
        };

        var result = _validator.TestValidate(voluntary);

        result.ShouldHaveValidationErrorFor(l => l.Name);
        result.ShouldHaveValidationErrorFor(l => l.Field);
    }

    #endregion Tests
}