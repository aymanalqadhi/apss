using APSS.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace APSS.Infrastructure.Repositores.EntityFramework;

public sealed class ApssDbContext : DbContext
{
    #region Ctors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="options">Options to be used to configure the database context</param>
    public ApssDbContext(DbContextOptions options) : base(options)
    {
    }

    #endregion Ctors

    #region Public properties

    /// <summary>
    /// Gets the accounts table
    /// </summary>
    public DbSet<Account> Accounts => Set<Account>();

    /// <summary>
    /// Gets the users set
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Gets the lands set
    /// </summary>
    public DbSet<Land> Lands => Set<Land>();

    /// <summary>
    /// Gets the animal groups set
    /// </summary>
    public DbSet<AnimalGroup> AnimalGroups => Set<AnimalGroup>();

    /// <summary>
    /// Gets the products set
    /// </summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>
    /// Gets the produt expenses set
    /// </summary>
    public DbSet<ProductExpense> ProductExpenses => Set<ProductExpense>();

    /// <summary>
    /// Gets the land products set
    /// </summary>
    public DbSet<LandProduct> LandProducts => Set<LandProduct>();

    /// <summary>
    /// Gets the seasons set
    /// </summary>
    public DbSet<Season> Sessions => Set<Season>();

    /// <summary>
    /// Gets the animal groups repositories
    /// </summary>
    public DbSet<AnimalProduct> AnimalProducts => Set<AnimalProduct>();

    /// <summary>
    /// Gets the surveys set
    /// </summary>
    public DbSet<Survey> Surveys => Set<Survey>();

    /// <summary>
    /// Gets the questions set
    /// </summary>
    public DbSet<Question> Questions => Set<Question>();

    /// <summary>
    /// Gets the multiple-choice questions set
    /// </summary>
    public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions => Set<MultipleChoiceQuestion>();

    /// <summary>
    /// Gets the multiple-choice questions' answer items set
    /// </summary>
    public DbSet<MultipleChoiceAnswerItem> MultipleChoiceAnswerItems => Set<MultipleChoiceAnswerItem>();

    /// <summary>
    /// Gets the logical questions set
    /// </summary>
    public DbSet<LogicalQuestion> LogicalQuestions => Set<LogicalQuestion>();

    /// <summary>
    /// Gets the text questions set
    /// </summary>
    public DbSet<TextQuestion> TextQuestions => Set<TextQuestion>();

    /// <summary>
    /// Gets the questions answers set
    /// </summary>
    public DbSet<QuestionAnswer> QuestionAnswers => Set<QuestionAnswer>();

    /// <summary>
    /// Gets the multiple-choice answers set
    /// </summary>
    public DbSet<MultipleChoiceQuestionAnswer> MultipleChoiceQuestionAnswers
        => Set<MultipleChoiceQuestionAnswer>();

    /// <summary>
    /// Gets the logical question answers set
    /// </summary>
    public DbSet<LogicalQuestionAnswer> LogicalQuestionAnswers => Set<LogicalQuestionAnswer>();

    /// <summary>
    /// Gets the text question answers set
    /// </summary>
    public DbSet<TextQuestionAnswer> TextQuestionAnswers => Set<TextQuestionAnswer>();

    /// <summary>
    /// Gets the survey entries set
    /// </summary>
    public DbSet<SurveyEntry> SurveyEntries => Set<SurveyEntry>();

    /// <summary>
    /// Gets the individuals set
    /// </summary>
    public DbSet<Individual> Individuals => Set<Individual>();

    /// <summary>
    /// Gets the skills set
    /// </summary>
    public DbSet<Skill> Skills => Set<Skill>();

    /// <summary>
    /// Gets the volantaries set
    /// </summary>
    public DbSet<Voluntary> Volantaries => Set<Voluntary>();

    /// <summary>
    /// Gets the families set
    /// </summary>
    public DbSet<Family> Families => Set<Family>();

    /// <summary>
    /// Gets the family individuals set
    /// </summary>
    public DbSet<FamilyIndividual> FamilyIndividuals => Set<FamilyIndividual>();

    /// <summary>
    /// Gets the logs set
    /// </summary>
    public DbSet<Log> Logs => Set<Log>();

    /// <summary>
    /// Gets the log tags table
    /// </summary>
    public DbSet<LogTag> LogTags => Set<LogTag>();

    #endregion Public properties
}