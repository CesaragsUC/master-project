using FluentMigrator;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Order.Infrastructure.Migrations;

[ExcludeFromCodeCoverage]
[Migration(202501230002)]
public class CreateOrderItemsTable : Migration
{
    public override void Up()
    {
        Console.WriteLine("Creating table OrdersItens.");

        if (!Schema.Table("OrderItens").Exists())
        {
            Create.Table("OrderItens")
                .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("OrderId").AsGuid().NotNullable()
                .WithColumn("ProductId").AsGuid().NotNullable()
                .WithColumn("ProductName").AsString().Nullable()
                .WithColumn("Quantity").AsInt32().NotNullable()
                .WithColumn("UnitPrice").AsDecimal().NotNullable()
                .WithColumn("TotalPrice").AsDecimal().NotNullable();

            
            Create.ForeignKey("FK_OrderItens_Orders")
                .FromTable("OrderItens").ForeignColumns("OrderId")
                .ToTable("Orders").PrimaryColumn("Id");
        }

        Console.WriteLine("Table OrderItens created.");
    }

    public override void Down()
    {
        Delete.ForeignKey("FK_OrderItens_Orders").OnTable("OrderItens");
        Delete.Table("OrderItens");
    }
}
