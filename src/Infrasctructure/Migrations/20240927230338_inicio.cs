using FluentMigrator;

namespace Infrastructure.Migrations;


[Migration(202409270001)]
public class Inicio : Migration
{
    public override void Up()
    {
        if (!Schema.Table("Produtos").Exists())
        {
            Create.Table("Produtos")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Nome").AsString().NotNullable()
            .WithColumn("Preco").AsDecimal().NotNullable()
            .WithColumn("Active").AsBoolean().NotNullable()
            .WithColumn("CreatAt").AsDateTimeOffset().Nullable(); // Utilizando DateTimeOffset para equivaler ao "timestamp with time zone"
        }

    }

    public override void Down()
    {
        Delete.Table("Produtos");
    }
}
