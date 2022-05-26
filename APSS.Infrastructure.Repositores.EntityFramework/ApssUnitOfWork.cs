using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Domain.Repositories;

namespace APSS.Infrastructure.Repositores.EntityFramework;

public sealed class ApssUnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
{
    #region Private fields

    private readonly ApssDbContext _ctx;

    #endregion

    #region Ctors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="ctx">The database context of the application</param>
    public ApssUnitOfWork(ApssDbContext ctx)
    {
        _ctx = ctx;
    }

    #endregion

    #region Public properties

    /// <inheritdoc/>
    public IRepository<User> Users => new Repository<User, UserValidator>(_ctx.Users);

    /// <inheritdoc/>
    public IRepository<PermissionInheritance> PermissionInheritances =>
        new Repository<PermissionInheritance, Validator<PermissionInheritance>>(_ctx.PermissionInheritances);

    /// <inheritdoc/>
    public IRepository<Land> Lands => new Repository<Land, LandValidator>(_ctx.Lands);

    /// <inheritdoc/>
    public IRepository<AnimalGroup> AnimalGroups =>
        new Repository<AnimalGroup, AnimalGroupValidator>(_ctx.AnimalGroups);

    /// <inheritdoc/>
    public IRepository<Product> Products => new Repository<Product, Validator<Product>>(_ctx.Products);

    /// <inheritdoc/>
    public IRepository<ProductExpense> ProductExpenses
        => new Repository<ProductExpense, ProductExpenseValidator>(_ctx.ProductExpenses);

    /// <inheritdoc/>
    public IRepository<LandProduct> LandProducts
        => new Repository<LandProduct, LandProductValidator>(_ctx.LandProducts);

    /// <inheritdoc/>
    public IRepository<Season> Sessions => new Repository<Season, SeasonValidator>(_ctx.Sessions);

    /// <inheritdoc/>
    public IRepository<AnimalProduct> AnimalProducts
        => new Repository<AnimalProduct, AnimalProductValidator>(_ctx.AnimalProducts);

    /// <inheritdoc/>
    public IRepository<Survey> Surveys => new Repository<Survey, SurveyValidator>(_ctx.Surveys);

    /// <inheritdoc/>
    public IRepository<Question> Questions => new Repository<Question, QuestionValidator>(_ctx.Questions);

    /// <inheritdoc/>
    public IRepository<MultipleChoiceQuestion> MultipleChoiceQuestions { get; }

    /// <inheritdoc/>
    public IRepository<MultipleChoiceAnswerItem> MultipleChoiceAnswerItems { get; }

    /// <inheritdoc/>
    public IRepository<LogicalQuestion> LogicalQuestions { get; }

    /// <inheritdoc/>
    public IRepository<TextQuestion> TextQuestions { get; }

    /// <inheritdoc/>
    public IRepository<QuestionAnswer> QuestionAnswers { get; }

    /// <inheritdoc/>
    public IRepository<MultipleChoiceQuestionAnswer> MultipleChoiceQuestionAnswers { get; }

    /// <inheritdoc/>
    public IRepository<LogicalQuestionAnswer> LogicalQuestionAnswers { get; }

    /// <inheritdoc/>
    public IRepository<TextQuestionAnswer> TextQuestionAnswers { get; }

    /// <inheritdoc/>
    public IRepository<SurveyEntry> SurveyEntries =>
        new Repository<SurveyEntry, Validator<SurveyEntry>>(_ctx.SurveyEntries);

    /// <inheritdoc/>
    public IRepository<Individual> Individuals =>
        new Repository<Individual, IndividualValidator>(_ctx.Individuals);

    /// <inheritdoc/>
    public IRepository<Skill> Skills => new Repository<Skill, SkillValidator>(_ctx.Skills);

    /// <inheritdoc/>
    public IRepository<Voluntary> Volantaries =>
        new Repository<Voluntary, VoluntaryValidator>(_ctx.Volantaries);

    /// <inheritdoc/>
    public IRepository<Family> Families => new Repository<Family, FamilyValidator>(_ctx.Families);

    /// <inheritdoc/>
    public IRepository<FamilyIndividual> FamiliesIndividuals { get; }

    /// <inheritdoc/>
    public IRepository<Log> Logs { get; }

    #endregion

    #region Public methods

    /// <inheritdoc/>
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => _ctx.SaveChangesAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<int> CommitAsync(
        IDatabaseTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        var ret = await CommitAsync();

        await transaction.CommitAsync(cancellationToken);

        return ret;
    }

    /// <inheritdoc/>
    public async Task<IDatabaseTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => new DatabaseTransaction(await _ctx.Database.BeginTransactionAsync(cancellationToken));

    /// <inheritdoc/>
    public void Dispose()
        => _ctx.Dispose();

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
        => _ctx.DisposeAsync();

    #endregion
}
