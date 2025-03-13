using FluentMigrator;
using System.Diagnostics.CodeAnalysis;

namespace Product.Infrastructure.Migrations;

/// <summary>
/// Não existe classe Categoria, então é criada a tabela Categoria via migração
/// </summary>
[ExcludeFromCodeCoverage]
[Migration(202409281048)]
public class Categoria : Migration
{
    public override void Down()
    {
        Delete.Table("Category");
    }

    public override void Up()
    {
        if (!Schema.Table("Category").Exists())
        {
            Create.Table("Category")
                .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Active").AsBoolean().NotNullable();
        }

    }
}