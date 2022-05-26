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

    #endregion
}
