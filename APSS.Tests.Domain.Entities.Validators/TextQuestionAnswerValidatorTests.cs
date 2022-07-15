using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Tests.Utils;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APSS.Tests.Domain.Entities.Validators;

[TestClass]
public class TextQuestionAnswerValidatorTests
{
    #region Private fields

    private readonly TextQuestionAnswerValidator _validator = new();

    #endregion Private fields

    #region Tests

    [TestMethod]
    public void ValidateTextQuestionAnswerShouldSucceed()
    {
        var textQuestion = new TextQuestionAnswer
        {
            Question = new TextQuestion { IsRequired = true },
            Answer = RandomGenerator.NextString(0xff)
        };

        Assert.IsTrue(_validator.Validate(textQuestion).IsValid);
    }

    [TestMethod]
    public void ValidateTextQuestionAnswerShouldFail()
    {
        var textQuestion = new TextQuestionAnswer
        {
            Question = new LogicalQuestion { IsRequired = true },
            Answer = ""
        };

        var result = _validator.TestValidate(textQuestion);

        result.ShouldHaveValidationErrorFor(a => a.Question);
        result.ShouldHaveValidationErrorFor(a => a.Answer);
    }

    #endregion Tests
}