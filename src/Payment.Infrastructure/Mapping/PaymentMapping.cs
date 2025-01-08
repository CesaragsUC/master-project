﻿
using Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Infrastructure.Mapping;

public class PaymentMapping : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Ignore(c => c.CreditCard);

        // 1 : N => Payment : Transaction
        builder.HasMany(c => c.Transactions)
            .WithOne(c => c.Payment)
            .HasForeignKey(c => c.PaymentId);

    }
}
