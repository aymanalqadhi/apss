using APSS.Domain.Entities;

namespace APSS.Domain.Repositories;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    #region Properties

    /// <summary>
    /// Gets the accounts repository
    /// </summary>
    IRepository<Account> Accounts { get; }

    /// <summary>
    /// Gets the animal groups repository
    /// </summary>
    IRepository<AnimalGroup> AnimalGroups { get; }

    /// <summary>
    /// Gets the animal groups repositories
    /// </summary>
    IRepository<AnimalProduct> AnimalProducts { get; }

    /// <summary>
    /// Gets the animal group products' units
    /// </summary>
    IRepository<AnimalProductUnit> AnimalProductUnits { get; }

    /// <summary>
    /// Gets the families repository
    /// </summary>
    IRepository<Family> Families { get; }

    /// <summary>
    /// Gets the family individuals repository
    /// </summary>
    IRepository<FamilyIndividual> FamilyIndividuals { get; }

    /// <summary>
    /// Gets the individuals repository
    /// </summary>
    IRepository<Individual> Individuals { get; }

    /// <summary>
    /// Gets the land products repository
    /// </summary>
    IRepository<LandProduct> LandProducts { get; }

    /// <summary>
    /// Gets the land products' units
    /// </summary>
    IRepository<LandProductUnit> LandProductUnits { get; }

    /// <summary>
    /// Gets the lands repository
    /// </summary>
    IRepository<Land> Lands { get; }

    /// <summary>
    /// Gets the logical question answers repository
    /// </summary>
    IRepository<LogicalQuestionAnswer> LogicalQuestionAnswers { get; }

    /// <summary>
    /// Gets the logical questions repository
    /// </summary>
    IRepository<LogicalQuestion> LogicalQuestions { get; }

    /// <summary>
    /// Gets the logs repository
    /// </summary>
    IRepository<Log> Logs { get; }

    /// <summary>
    /// Gets the log tags repository
    /// </summary>
    IRepository<LogTag> LogTags { get; }

    /// <summary>
    /// Gets the multiple-choice questions' answer items repository
    /// </summary>
    IRepository<MultipleChoiceAnswerItem> MultipleChoiceAnswerItems { get; }

    /// <summary>
    /// Gets the multiple-choice question answers repository
    /// </summary>
    IRepository<MultipleChoiceQuestionAnswer> MultipleChoiceQuestionAnswers { get; }

    /// <summary>
    /// Gets the multiple-choice questions repository
    /// </summary>
    IRepository<MultipleChoiceQuestion> MultipleChoiceQuestions { get; }

    /// <summary>
    /// Gets the produt expenses repository
    /// </summary>
    IRepository<ProductExpense> ProductExpenses { get; }

    /// <summary>
    /// Gets the products repository
    /// </summary>
    IRepository<Product> Products { get; }

    /// <summary>
    /// Gets the question answers repository
    /// </summary>
    IRepository<QuestionAnswer> QuestionAnswers { get; }

    /// <summary>
    /// Gets the questions repository
    /// </summary>
    IRepository<Question> Questions { get; }

    /// <summary>
    /// Gets the seasons repository
    /// </summary>
    IRepository<Season> Sessions { get; }

    /// <summary>
    /// Gets the skills repository
    /// </summary>
    IRepository<Skill> Skills { get; }

    /// <summary>
    /// Gets the survey entries repository
    /// </summary>
    IRepository<SurveyEntry> SurveyEntries { get; }

    /// <summary>
    /// Gets the surveys repository
    /// </summary>
    IRepository<Survey> Surveys { get; }

    /// <summary>
    /// Gets the text question answers repository
    /// </summary>
    IRepository<TextQuestionAnswer> TextQuestionAnswers { get; }

    /// <summary>
    /// Gets the text questions repository
    /// </summary>
    IRepository<TextQuestion> TextQuestions { get; }

    /// <summary>
    /// Gets the users repository
    /// </summary>
    IRepository<User> Users { get; }

    /// <summary>
    /// Gets the volantaries repository
    /// </summary>
    IRepository<Voluntary> Volantaries { get; }

    #endregion Properties

    #region Public Methods

    /// <summary>
    /// Asynchronously begins a transaction
    /// </summary>
    /// <param name="cancellationToken">The task cancellation token</param>
    /// <returns></returns>
    Task<IAsyncDatabaseTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously commits changes to data backend
    /// </summary>
    /// <param name="cancellationToken">The task cancellation token</param>
    /// <returns>The affected records count</returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously commits changes to data backend
    /// </summary>
    /// <param name="cancellationToken">The task cancellation token</param>
    /// <returns>The affected records count</returns>
    Task<int> CommitAsync(IAsyncDatabaseTransaction transaction, CancellationToken cancellationToken = default);

    #endregion Public Methods
}