using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APSS.Tests.Domain.Entities.Validators;

[TestClass]
public class LogicalQuestionAnswerValidatorTests
{
    #region Private fields

    private readonly LogicalQuestionAnswerValidator _validator = new();

    #endregion Private fields

    #region Tests

    [TestMethod]
    public void ValidateLogicalQuestionAnswerShouldSucceed()
    {
        var logicalQuestionAnswer = new LogicalQuestionAnswer
        {
            Question = new LogicalQuestion { IsRequired = true },
            Answer = true
        };

        Assert.IsTrue(_validator.Validate(logicalQuestionAnswer).IsValid);
    }

    [TestMethod]
    public void ValidateLogicalQuestionAnswerShouldFail()
    {
        var logicalQuestionAnswer = new LogicalQuestionAnswer
        {
            Question = new MultipleChoiceQuestion { IsRequired = true },
            Answer = null
        };

        var result = _validator.TestValidate(logicalQuestionAnswer);

        result.ShouldHaveValidationErrorFor(a => a.Question);
        result.ShouldHaveValidationErrorFor(a => a.Answer);
    }

    #endregion Tests
}