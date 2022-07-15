using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Tests.Utils;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace APSS.Tests.Domain.Entities.Validators;

[TestClass]
public class ProductExpenseValidatorTests
{
    #region Private fields

    private readonly ProductExpenseValidator _validator = new();

    #endregion Private fields

    #region Tests

    [TestMethod]
    public void ValidateProductExpenseShouldSucceed()
    {
        var expense = new ProductExpense
        {
            Price = Convert.ToDecimal(RandomGenerator.NextDouble(0, 1_000_000))
        };

        Assert.IsTrue(_validator.Validate(expense).IsValid);
    }

    [TestMethod]
    public void ValidateProductExpenseShouldFail()
    {
        var expense = new ProductExpense
        {
            Price = -2
        };

        var result = _validator.TestValidate(expense);

        result.ShouldHaveValidationErrorFor(l => l.Price);
    }

    #endregion Tests
}