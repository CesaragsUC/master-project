using FluentMigrator;
using System.Collections.Generic;

namespace Infrastructure.Migrations;

/// <summary>
/// Não existe classe Categoria, então é criada a tabela Categoria via migração
/// </summary>

[Migration(202409281048)]
public class Categoria : Migration
{
    public override void Down()
    {
        Delete.Table("Categoria");
    }

    public override void Up()
    {
        if (!Schema.Table("Categoria").Exists())
        {
            Create.Table("Categoria")
                .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("Nome").AsString().NotNullable()
                .WithColumn("Ativo").AsBoolean().NotNullable();
        }

    }
}

//INSERT INTO "Categoria" ("Id", "Nome", "Ativo") VALUES('b8f8c9a8-8e7c-4e16-b0a1-4e8f715e5891', 'Eletrônicos', true);
//INSERT INTO "Categoria" ("Id", "Nome", "Ativo") VALUES('c4f9e9b4-9e3d-4f71-a0bc-1c7f5f2a1b7a', 'Roupas', true);
//INSERT INTO "Categoria" ("Id", "Nome", "Ativo") VALUES('6c39cf0f-2b2f-4121-9e7b-6f90567627e7', 'Calçados', true);
//INSERT INTO "Categoria" ("Id", "Nome", "Ativo") VALUES('29cf13d3-3b8e-4b6a-9ad7-c91e7f45b7b8', 'Esportes', true);
//INSERT INTO "Categoria" ("Id", "Nome", "Ativo") VALUES('46d3b08d-4b5e-45c9-9613-b1e2c3484d09', 'Livros', true);