using FluentMigrator;
using System.Diagnostics.CodeAnalysis;

namespace Order.Infrastructure.Migrations;

[ExcludeFromCodeCoverage]
[Migration(202501230001)]
public class CreateOrderTable : Migration
{
    public override void Up()
    {
        Console.WriteLine("Creating table Orders.");

        if (!Schema.Table("Orders").Exists())
        {
            Create.Table("Orders")

            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("CustomerId").AsGuid().NotNullable()
            .WithColumn("Name").AsString().NotNullable()
            .WithColumn("TotalAmount").AsDecimal().NotNullable()
            .WithColumn("Status").AsInt32().NotNullable()
            .WithColumn("CreatedDate").AsDateTimeOffset().NotNullable()
            .WithColumn("UpdatedDate").AsDateTimeOffset().Nullable()
            .WithColumn("IsDeleted").AsBoolean().Nullable()
            .WithColumn("PaymentToken").AsAnsiString().NotNullable();
        }
        Console.WriteLine("Table Orders created.");
    }

    public override void Down()
    {
        Delete.Table("Orders");
    }
}