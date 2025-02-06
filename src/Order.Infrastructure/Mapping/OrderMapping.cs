using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infrastructure.Mapping;

public class OrderMapping : IEntityTypeConfiguration<Domain.Entities.Order>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Order> builder)
    {
        builder.HasMany(o => o.Items)
        .WithOne(i => i.Order)
        .HasForeignKey(i => i.OrderId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}
