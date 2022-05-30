using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Tests.Utils;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentValidation.TestHelper;

namespace APSS.Tests.Domain.Entities.Validators;

[TestClass]
public class SurveyValidatorTests
{
    #region Private fields
    
    private readonly SurveyValidator _validator = new();

    #endregion

    #region Tests

    [TestMethod]
    public void ValidateSurveyShouldSucceed()
    {
        var survey = new Survey
        {
            Name = RandomGenerator.NextString(0xff),
            CreatedBy = new User { AccessLevel = AccessLevel.Group},
        };

        Assert.IsTrue(_validator.Validate(survey).IsValid);
    }

    [TestMethod]
    public void ValidateSurveyShouldFail()
    {
        var survey = new Survey 
        {
            Name = "",
            CreatedBy = new User { AccessLevel = AccessLevel.Farmer},
        };

        var result = _validator.TestValidate(survey);

        result.ShouldHaveValidationErrorFor(l => l.Name);
        result.ShouldHaveValidationErrorFor(l => l.CreatedBy.AccessLevel);
    }

    #endregion
}
