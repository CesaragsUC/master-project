using FluentMigrator;

namespace Infrastructure.Migrations;

[Migration(202411071326)]
public class AdicionadoImagem : Migration
{
    public override void Down()
    {
        // Remove apenas a coluna 'ImageUri' em vez de deletar a tabela inteira
        if (Schema.Table("Produtos").Column("ImageUri").Exists())
        {
            Delete.Column("ImageUri").FromTable("Produtos");
        }
    }

    public override void Up()
    {
        if (Schema.Table("Produtos").Exists())
        {
            Console.WriteLine("Tabela 'Produtos' existe.");
            if (!Schema.Table("Produtos").Column("ImageUri").Exists())
            {
                Console.WriteLine("Adicionando coluna 'ImageUri'...");
                Alter.Table("Produtos").AddColumn("ImageUri").AsString();
                Console.WriteLine("Coluna 'ImageUri' adicionada.");
            }
            else
            {
                Console.WriteLine("Coluna 'ImageUri' já existe.");
            }
        }
        else
        {
            Console.WriteLine("Tabela 'Produtos' não existe.");
        }
    }

}
