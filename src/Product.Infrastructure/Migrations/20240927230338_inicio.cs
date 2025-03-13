using FluentMigrator;
using System.Diagnostics.CodeAnalysis;

namespace Product.Infrastructure.Migrations;

[ExcludeFromCodeCoverage]
[Migration(202409270001)]
public class Inicio : Migration
{
    public override void Up()
    {
        Console.WriteLine("Creating table Products.");

        if (!Schema.Table("Products").Exists())
        {
            Create.Table("Products")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Name").AsString().NotNullable()
            .WithColumn("Price").AsDecimal().NotNullable()
            .WithColumn("Active").AsBoolean().NotNullable()
            .WithColumn("CreatAt").AsDateTimeOffset().Nullable(); // Utilizando DateTimeOffset para equivaler ao "timestamp with time zone"
        }
        Console.WriteLine("Table Products created.");
    }

    public override void Down()
    {
        Delete.Table("Products");
    }
}
