using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Tests.Utils;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace APSS.Tests.Domain.Entities.Validators;

[TestClass]
public class MultipleChoiceQuestionValidatorTests
{
    #region Private fields

    private readonly MultipleChoiceQuestionValidator _validator = new();

    #endregion Private fields

    #region Tests

    [TestMethod]
    public void ValidateMultipleChoiceQuestionShouldSucceed()
    {
        var multiple = new MultipleChoiceQuestion
        {
            CandidateAnswers = Enumerable.Range(2, RandomGenerator.NextInt(2, 6))
                .Select(i => new MultipleChoiceAnswerItem { Value = RandomGenerator.NextString(0xff) })
                .ToList()
        };

        Assert.IsTrue(_validator.Validate(multiple).IsValid);
    }

    [TestMethod]
    public void ValidateMultipleChoiceQuestionShouldFail()
    {
        var multiple = new MultipleChoiceQuestion
        {
            CandidateAnswers = Enumerable.Range(0, 1)
                .Select(i => new MultipleChoiceAnswerItem { Value = RandomGenerator.NextString(0xff) })
                .ToList()
        };

        var result = _validator.TestValidate(multiple);

        result.ShouldHaveValidationErrorFor(l => l.CandidateAnswers.Count);
    }

    #endregion Tests
}