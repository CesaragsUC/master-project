using FluentMigrator;

namespace Infrastructure.Migrations;

[Migration(202411071418)]
public class RemoverColunaTeste : Migration
{
    public override void Up()
    {
        if (Schema.Table("Produtos").Exists())
        {
            Console.WriteLine("Tabela 'Produtos' existe.");
            if (Schema.Table("Produtos").Column("ColunaTeste").Exists())
            {
                Console.WriteLine("Removendo coluna 'ColunaTeste'...");
                Delete.Column("ColunaTeste").FromTable("Produtos");
                Console.WriteLine("Coluna 'ColunaTeste' removida.");
            }
            else
            {
                Console.WriteLine("Coluna 'ColunaTeste' não existe.");
            }
        }
        else
        {
            Console.WriteLine("Tabela 'Produtos' não existe.");
        }
    }

    public override void Down()
    {
        if (Schema.Table("Produtos").Exists())
        {
            Console.WriteLine("Adicionando coluna 'ColunaTeste'...");
            Alter.Table("Produtos").AddColumn("ColunaTeste").AsString().Nullable();
            Console.WriteLine("Coluna 'ColunaTeste' adicionada.");
        }
    }
}


