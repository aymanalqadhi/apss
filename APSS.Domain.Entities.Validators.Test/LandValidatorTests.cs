using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Tests.Utils;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APSS.Tests.Domain.Entities.Validators;

[TestClass]
public class LandValidatorTests
{
    #region Private fields

    private readonly LandValidator _validator = new();

    #endregion Private fields

    #region Tests

    [TestMethod]
    public void ValidateLandShouldSucceed()
    {
        var land = new Land
        {
            Name = RandomGenerator.NextString(0xff),
            OwnedBy = new User { AccessLevel = AccessLevel.Farmer }
        };

        Assert.IsTrue(_validator.Validate(land).IsValid);
    }

    [TestMethod]
    public void ValidateLandShouldFail()
    {
        var land = new Land
        {
            Name = "",
            OwnedBy = new User { AccessLevel = AccessLevel.Group }
        };

        var result = _validator.TestValidate(land);

        result.ShouldHaveValidationErrorFor(a => a.Name);
        result.ShouldHaveValidationErrorFor(a => a.OwnedBy.AccessLevel);
    }

    #endregion Tests
}