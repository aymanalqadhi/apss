using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Domain.Repositories;

namespace APSS.Infrastructure.Repositores.EntityFramework;

public sealed class ApssUnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
{
    #region Fields

    private readonly ApssDbContext _ctx;

    #endregion Fields

    #region Public Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="ctx">The database context of the application</param>
    public ApssUnitOfWork(ApssDbContext ctx)
    {
        _ctx = ctx;
    }

    #endregion Public Constructors

    #region Properties

    /// <inheritdoc/>
    public IRepository<Account> Accounts => new Repository<Account, AccountValidator>(_ctx.Accounts);

    /// <inheritdoc/>
    public IRepository<AnimalGroup> AnimalGroups =>
        new Repository<AnimalGroup, AnimalGroupValidator>(_ctx.AnimalGroups);

    /// <inheritdoc/>
    public IRepository<AnimalProduct> AnimalProducts
        => new Repository<AnimalProduct, AnimalProductValidator>(_ctx.AnimalProducts);

    /// <inheritdoc/>
    public IRepository<AnimalProductUnit> AnimalProductUnits
        => new Repository<AnimalProductUnit, AnimalProductUnitValidator>(_ctx.AnimalProductUnits);

    /// <inheritdoc/>
    public IRepository<Family> Families => new Repository<Family, FamilyValidator>(_ctx.Families);

    /// <inheritdoc/>
    public IRepository<FamilyIndividual> FamilyIndividuals =>
        new Repository<FamilyIndividual, Validator<FamilyIndividual>>(_ctx.FamilyIndividuals);

    /// <inheritdoc/>
    public IRepository<Individual> Individuals =>
        new Repository<Individual, IndividualValidator>(_ctx.Individuals);

    /// <inheritdoc/>
    public IRepository<LandProduct> LandProducts
        => new Repository<LandProduct, LandProductValidator>(_ctx.LandProducts);

    /// <inheritdoc/>
    public IRepository<LandProductUnit> LandProductUnits
        => new Repository<LandProductUnit, LandProductUnitValidator>(_ctx.LandProductUnits);

    /// <inheritdoc/>
    public IRepository<Land> Lands => new Repository<Land, LandValidator>(_ctx.Lands);

    /// <inheritdoc/>
    public IRepository<LogicalQuestionAnswer> LogicalQuestionAnswers
        => new Repository<LogicalQuestionAnswer, Validator<LogicalQuestionAnswer>>(_ctx.LogicalQuestionAnswers);

    /// <inheritdoc/>
    public IRepository<LogicalQuestion> LogicalQuestions =>
        new Repository<LogicalQuestion, Validator<LogicalQuestion>>(_ctx.LogicalQuestions);

    /// <inheritdoc/>
    public IRepository<Log> Logs => new Repository<Log, LogValidator>(_ctx.Logs);

    /// <inheritdoc/>
    public IRepository<LogTag> LogTags => new Repository<LogTag, LogTagValidator>(_ctx.LogTags);

    /// <inheritdoc/>
    public IRepository<MultipleChoiceAnswerItem> MultipleChoiceAnswerItems =>
        new Repository<MultipleChoiceAnswerItem, MultipleChoiceAnswerItemValidator>(_ctx.MultipleChoiceAnswerItems);

    /// <inheritdoc/>
    public IRepository<MultipleChoiceQuestionAnswer> MultipleChoiceQuestionAnswers =>
        new Repository<MultipleChoiceQuestionAnswer, MultipleChoiceQuestionAnswerValidator>(_ctx.MultipleChoiceQuestionAnswers);

    /// <inheritdoc/>
    public IRepository<MultipleChoiceQuestion> MultipleChoiceQuestions =>
        new Repository<MultipleChoiceQuestion, MultipleChoiceQuestionValidator>(_ctx.MultipleChoiceQuestions);

    /// <inheritdoc/>
    public IRepository<ProductExpense> ProductExpenses
        => new Repository<ProductExpense, ProductExpenseValidator>(_ctx.ProductExpenses);

    /// <inheritdoc/>
    public IRepository<Product> Products => new Repository<Product, Validator<Product>>(_ctx.Products);

    /// <inheritdoc/>
    public IRepository<QuestionAnswer> QuestionAnswers =>
        new Repository<QuestionAnswer, Validator<QuestionAnswer>>(_ctx.QuestionAnswers);

    /// <inheritdoc/>
    public IRepository<Question> Questions => new Repository<Question, QuestionValidator>(_ctx.Questions);

    /// <inheritdoc/>
    public IRepository<Season> Sessions => new Repository<Season, SeasonValidator>(_ctx.Sessions);

    /// <inheritdoc/>
    public IRepository<Skill> Skills => new Repository<Skill, SkillValidator>(_ctx.Skills);

    /// <inheritdoc/>
    public IRepository<SurveyEntry> SurveyEntries =>
        new Repository<SurveyEntry, Validator<SurveyEntry>>(_ctx.SurveyEntries);

    /// <inheritdoc/>
    public IRepository<Survey> Surveys => new Repository<Survey, SurveyValidator>(_ctx.Surveys);

    /// <inheritdoc/>
    public IRepository<TextQuestionAnswer> TextQuestionAnswers =>
        new Repository<TextQuestionAnswer, TextQuestionAnswerValidator>(_ctx.TextQuestionAnswers);

    /// <inheritdoc/>
    public IRepository<TextQuestion> TextQuestions =>
        new Repository<TextQuestion, Validator<TextQuestion>>(_ctx.TextQuestions);

    /// <inheritdoc/>
    public IRepository<User> Users => new Repository<User, UserValidator>(_ctx.Users);

    /// <inheritdoc/>
    public IRepository<Voluntary> Volantaries =>
        new Repository<Voluntary, VoluntaryValidator>(_ctx.Volantaries);

    #endregion Properties

    #region Public Methods

    /// <inheritdoc/>
    public async Task<IAsyncDatabaseTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => new AsyncDatabaseTransaction(await _ctx.Database.BeginTransactionAsync(cancellationToken));

    /// <inheritdoc/>
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => _ctx.SaveChangesAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<int> CommitAsync(
        IAsyncDatabaseTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        var ret = await CommitAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return ret;
    }

    /// <inheritdoc/>
    public void Dispose()
        => _ctx.Dispose();

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
        => _ctx.DisposeAsync();

    #endregion Public Methods
}