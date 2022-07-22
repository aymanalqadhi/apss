using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using APSS.Domain.Entities;

namespace APSS.Infrastructure.Repositories.EntityFramework.Configuration;

public sealed class AccountTypeConfiguration : IEntityTypeConfiguration<Account>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        // relationships
        builder
            .HasOne(a => a.User)
            .WithMany(a => a.Accounts)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(a => a.AddedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}