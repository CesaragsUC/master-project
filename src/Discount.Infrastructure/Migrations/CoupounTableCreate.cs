using FluentMigrator;
using System.Diagnostics.CodeAnalysis;

namespace Discount.Infrastructure.Migrations;

[ExcludeFromCodeCoverage]

// ano-mes-dia
//0001 É usado para garantir a ordem de execução das migrações no banco.
[Migration(202501100001)]
public class CoupounTableCreate : Migration
{
    public override void Up()
    {
        Console.WriteLine("Creating table Coupons.");

        if (!Schema.Table("Coupons").Exists())
        {
            Create.Table("Coupons")

            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Code").AsString().NotNullable()
            .WithColumn("Type").AsInt32().NotNullable()
            .WithColumn("Value").AsDecimal().NotNullable()
            .WithColumn("MinValue").AsDecimal().NotNullable()
            .WithColumn("StartDate").AsDateTimeOffset().NotNullable()
            .WithColumn("EndDate").AsDateTimeOffset().NotNullable()
            .WithColumn("Active").AsBoolean().NotNullable()
            .WithColumn("MaxUse").AsInt32().NotNullable()
            .WithColumn("TotalUse").AsInt32().Nullable()
            .WithColumn("CreateAt").AsDateTimeOffset().NotNullable()
            .WithColumn("UpdatedAt").AsDateTimeOffset().Nullable(); // Utilizando DateTimeOffset para equivaler ao "timestamp with time zone"
        }
        Console.WriteLine("Table Coupons created.");
    }

    public override void Down()
    {
        Delete.Table("Coupons");
    }
}
