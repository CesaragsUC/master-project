using FluentMigrator;

namespace Infrastructure.Migrations;

[Migration(202411071326)]
public class AdicionadoImagem : Migration
{
    public override void Down()
    {
        // Remove apenas a coluna 'ImageUri' em vez de deletar a tabela inteira
        if (Schema.Table("Products").Column("ImageUri").Exists())
        {
            Delete.Column("ImageUri").FromTable("Products");
        }
    }

    public override void Up()
    {
        if (Schema.Table("Products").Exists())
        {

            if (!Schema.Table("Products").Column("ImageUri").Exists())
            {
                Alter.Table("Products").AddColumn("ImageUri").AsString();
            }
        }
    }

}
