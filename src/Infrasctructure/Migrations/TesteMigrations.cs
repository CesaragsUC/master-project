using FluentMigrator;

namespace Infrastructure.Migrations;

[Migration(202411070836)]
public class TesteMigrations : Migration
{
    public override void Down()
    {
        // Remove apenas a coluna 'ImageUri' em vez de deletar a tabela inteira
        if (Schema.Table("Produtos").Column("ImageUri").Exists())
        {
            Delete.Column("ColunaTeste").FromTable("Produtos");
        }
    }

    public override void Up()
    {
        if (Schema.Table("Produtos").Exists())
        {
            Console.WriteLine("Tabela 'Produtos' existe.");
            if (!Schema.Table("Produtos").Column("ColunaTeste").Exists())
            {
                Console.WriteLine("Adicionando coluna 'ColunaTeste'...");
                Alter.Table("Produtos").AddColumn("ColunaTeste").AsString();
                Console.WriteLine("Coluna 'ColunaTeste' adicionada.");
            }
            else
            {
                Console.WriteLine("Coluna 'ColunaTeste' já existe.");
            }
        }
        else
        {
            Console.WriteLine("Tabela 'Produtos' não existe.");
        }
    }

}
