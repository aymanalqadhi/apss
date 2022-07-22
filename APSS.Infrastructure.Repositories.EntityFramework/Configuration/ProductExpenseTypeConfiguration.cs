using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using APSS.Domain.Entities;

namespace APSS.Infrastructure.Repositories.EntityFramework.Configuration;

public sealed class ProductExpenseTypeConfiguration : IEntityTypeConfiguration<ProductExpense>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<ProductExpense> builder)
    {
        builder
            .Property(s => s.Price)
            .HasPrecision(18, 6);
    }
}