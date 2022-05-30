using APSS.Domain.Entities;
using APSS.Domain.Entities.Validators;
using APSS.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

namespace APSS.Infrastructure.Repositores.EntityFramework;

public sealed class Repository<T, Validator> : IRepository<T>
    where T : AuditableEntity
    where Validator : Validator<T>, new()
{
    #region Private fields

    private readonly DbSet<T> _set;
    private readonly Validator _validator;

    #endregion

    #region Ctors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="set">the database set that backs the repository</param>
    public Repository(DbSet<T> set)
    {
        _set = set;
        _validator = new Validator();
    }

    #endregion

    #region Public methods

    /// <inheritdoc/>
    public void Add(params T[] entites)
    {
        foreach (var entity in entites)
        {
            var res = _validator.Validate(entity);

            if (!res.IsValid)
                throw new ValidationAggregateException(res.Errors);

            entity.CreatedAt = DateTime.Now;
            entity.ModifiedAt = DateTime.Now;
        }

        _set.AddRange(entites);
    }

    /// <inheritdoc/>
    public void Update(T entity)
    {
        var res = _validator.Validate(entity);

        if (!res.IsValid)
            throw new ValidationAggregateException(res.Errors);

        entity.ModifiedAt = DateTime.Now;

        _set.Update(entity);
    }

    /// <inheritdoc/>
    public void Remove(T entity)
        => _set.Remove(entity);

    /// <inheritdoc/>
    public IQueryBuilder<T> Query()
        => new QueryBuilder<T>(_set.AsQueryable());

    #endregion
}
