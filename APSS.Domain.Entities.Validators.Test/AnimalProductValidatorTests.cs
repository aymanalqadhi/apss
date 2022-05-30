using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Tests.Utils;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentValidation.TestHelper;

namespace APSS.Tests.Domain.Entities.Validators;

[TestClass]
public class AnimalProductValidatorTests
{
    #region Private fields

    private readonly AnimalProductValidator _validator = new();

    #endregion

    #region Tests

    [TestMethod]
    public void ValidateAnimalProductShouldSucceed()
    {
        var animalProduct = new AnimalProduct
        {
            Name = RandomGenerator.NextString(0xff),
            Quantity = RandomGenerator.NextInt(1),
        };

        Assert.IsTrue(_validator.Validate(animalProduct).IsValid);
    }

    [TestMethod]
    public void ValidateAnimalproductShouldFail()
    {
        var animalProduct = new AnimalProduct
        {
            Name = "",
            Quantity = -2,
        };

        var result = _validator.TestValidate(animalProduct);

        result.ShouldHaveValidationErrorFor(a => a.Name);
        result.ShouldHaveValidationErrorFor(a => a.Quantity);
    }

    #endregion
}
