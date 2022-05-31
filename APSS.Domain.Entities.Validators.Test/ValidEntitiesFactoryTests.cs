using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APSS.Tests.Domain.Entities.Validators;

[TestClass]
public class ValidEntitiesFactoryTests
{
    #region Tests

    [TestMethod]
    public void ValidateCreateValidUserShouldSucceed()
    {
        var validator = new UserValidator();
        var user = ValidEntitiesFactory.CreateValidUser(AccessLevel.Farmer);

        Assert.IsTrue(validator.Validate(user).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidAnimalGroupShouldSucceed()
    {
        var validator = new AnimalGroupValidator();
        var animalGroup = ValidEntitiesFactory.CreateValidAnimalGroup();

        Assert.IsTrue(validator.Validate(animalGroup).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidAnimalProductShouldSucceed()
    {
        var validator = new AnimalProductValidator();
        var animalProduct = ValidEntitiesFactory.CreateValidAnimalProduct();

        Assert.IsTrue(validator.Validate(animalProduct).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidFamilyShouldSucceed()
    {
        var validator = new FamilyValidator();
        var family = ValidEntitiesFactory.CreateValidFamily();

        Assert.IsTrue(validator.Validate(family).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidIndividualShouldSucceed()
    {
        var validator = new IndividualValidator();
        var individual = ValidEntitiesFactory.CreateValidIndividual();

        Assert.IsTrue(validator.Validate(individual).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidLandShouldSucceed()
    {
        var validator = new LandValidator();
        var land = ValidEntitiesFactory.CreateValidLand();

        Assert.IsTrue(validator.Validate(land).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidLandProductShouldSucceed()
    {
        var validator = new LandProductValidator();
        var landProduct = ValidEntitiesFactory.CreateValidLandProduct();

        Assert.IsTrue(validator.Validate(landProduct).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidSeasonShouldSucceed()
    {
        var validator = new SeasonValidator();
        var season = ValidEntitiesFactory.CreateValidSeason();

        Assert.IsTrue(validator.Validate(season).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidLogShouldSucceed()
    {
        var validator = new LogValidator();
        var log = ValidEntitiesFactory.CreateValidLog();

        Assert.IsTrue(validator.Validate(log).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidProductExpenseShouldSucceed()
    {
        var validator = new ProductExpenseValidator();
        var productExpense = ValidEntitiesFactory.CreateValidProductExpense();

        Assert.IsTrue(validator.Validate(productExpense).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidSkillShouldSucceed()
    {
        var validator = new SkillValidator();
        var skill = ValidEntitiesFactory.CreateValidSkill();

        Assert.IsTrue(validator.Validate(skill).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidVoluntaryShouldSucceed()
    {
        var validator = new VoluntaryValidator();
        var voluntary = ValidEntitiesFactory.CreateValidVoluntary();

        Assert.IsTrue(validator.Validate(voluntary).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidSurveyShouldSucceed()
    {
        var validator = new SurveyValidator();
        var survey = ValidEntitiesFactory.CreateValidSurvey();

        Assert.IsTrue(validator.Validate(survey).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidMultipleChoiceAnswerItemShouldSucceed()
    {
        var validator = new MultipleChoiceAnswerItemValidator();
        var item = ValidEntitiesFactory.CreateValidMultipleChoiceAnswerItem();

        Assert.IsTrue(validator.Validate(item).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidMultipleChoiceQuestionShouldSucceed()
    {
        var validator = new MultipleChoiceQuestionValidator();
        var multiple = ValidEntitiesFactory.CreateValidMultipleChoiceQuestion(true);

        Assert.IsTrue(validator.Validate(multiple).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidLogicalQuestionAnswerShouldSucceed()
    {
        var validator = new LogicalQuestionAnswerValidator();
        var logical = ValidEntitiesFactory.CreateValidLogicalQuestionAnswer();

        Assert.IsTrue(validator.Validate(logical).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidTextQuestionAnswerShouldSucceed()
    {
        var validator = new TextQuestionAnswerValidator();
        var text = ValidEntitiesFactory.CreateValidTextQuestionAnswer();

        Assert.IsTrue(validator.Validate(text).IsValid);
    }

    [TestMethod]
    public void ValidateCreateValidMultipleChoiceQuestionAnswerShouldSucceed()
    {
        var validator = new MultipleChoiceQuestionAnswerValidator();
        var multiple = ValidEntitiesFactory.CreateValidMultipleChoiceQuestionAnswer();

        Assert.IsTrue(validator.Validate(multiple).IsValid);
    }

    #endregion Tests
}