using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Tests.Utils;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace APSS.Tests.Domain.Entities.Validators;

[TestClass]
public class MultipleChoiceQuestionAnswerValidatorTests
{
    #region Private fields

    private readonly MultipleChoiceQuestionAnswerValidator _validator = new();

    #endregion Private fields

    #region Tests

    [TestMethod]
    public void ValidateMultipleChoiceQuestionAnswerShouldSucceed()
    {
        var question = new MultipleChoiceQuestion
        {
            Index = (uint)RandomGenerator.NextInt(0),
            Text = RandomGenerator.NextString(0xff),
            IsRequired = true,
            CandidateAnswers = Enumerable.Range(2, RandomGenerator.NextInt(2, 6))
                .Select(i => new MultipleChoiceAnswerItem { Value = RandomGenerator.NextString(0xff) })
                .ToList(),
            CanMultiSelect = RandomGenerator.NextBool(),
        };

        var multiple = new MultipleChoiceQuestionAnswer
        {
            Question = new MultipleChoiceQuestion { IsRequired = true },
            Answers = question.CanMultiSelect
                ? question
                    .CandidateAnswers
                    .Take(RandomGenerator.NextInt(2, question.CandidateAnswers.Count))
                    .ToList()
                : new List<MultipleChoiceAnswerItem> { question.CandidateAnswers.First() },
        };

        Assert.IsTrue(_validator.Validate(multiple).IsValid);
    }

    [TestMethod]
    public void ValidateMultipleChoiceQuestionAnswerShouldFail()
    {
        var multiple = new MultipleChoiceQuestionAnswer
        {
            Question = new MultipleChoiceQuestion { IsRequired = true },
            Answers = new List<MultipleChoiceAnswerItem>()
        };

        var result = _validator.TestValidate(multiple);

        result.ShouldHaveValidationErrorFor(a => a.Answers.Count);

        var multipleWithInvalidQuestion = new MultipleChoiceQuestionAnswer
        {
            Question = new TextQuestion { IsRequired = true }
        };

        var secondResult = _validator.TestValidate(multipleWithInvalidQuestion);

        secondResult.ShouldHaveValidationErrorFor(a => a.Question);
    }

    #endregion Tests
}