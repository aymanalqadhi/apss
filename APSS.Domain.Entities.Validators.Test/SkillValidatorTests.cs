﻿using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Tests.Utils;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentValidation.TestHelper;

namespace APSS.Tests.Domain.Entities.Validators;

[TestClass]
public class SkillValidatorTests
{
    #region Private fields
    
    private readonly SkillValidator _validator = new();

    #endregion

    #region Tests

    [TestMethod]
    public void ValidateSkillShouldSucceed()
    {
        var skill = new Skill
        {
            Name = RandomGenerator.NextString(0xff),
        };

        Assert.IsTrue(_validator.Validate(skill).IsValid);
    }

    [TestMethod]
    public void ValidateSkillShouldFail()
    {
        var skill = new Skill 
        {
            Name = "",
        };

        var result = _validator.TestValidate(skill);

        result.ShouldHaveValidationErrorFor(l => l.Name);
    }

    #endregion
}
