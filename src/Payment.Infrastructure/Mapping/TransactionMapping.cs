using Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Infrastructure.Mapping;

[ExcludeFromCodeCoverage]
public class TransactionMapping : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(c => c.Id);

        // 1 : N => Payment : Transaction
        builder.HasOne(c => c.Payment)
            .WithMany(c => c.Transactions);
    }
}