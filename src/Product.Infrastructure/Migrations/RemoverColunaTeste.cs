using FluentMigrator;
using System.Diagnostics.CodeAnalysis;

namespace Product.Infrastructure.Migrations;

[ExcludeFromCodeCoverage]
[Migration(202411071418)]
public class RemoverColunaTeste : Migration
{
    public override void Up()
    {
        if (Schema.Table("Products").Exists())
        {
            if (Schema.Table("Products").Column("Quantity").Exists())
            {
                Delete.Column("Quantity").FromTable("Products");
            }
        }
    }

    public override void Down()
    {
        if (Schema.Table("Products").Exists())
        {
            Alter.Table("Products").AddColumn("Quantity").AsString().Nullable();
        }
    }
}


