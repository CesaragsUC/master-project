using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Infrastructure.Migrations;

[ExcludeFromCodeCoverage]
[Migration(202501241001)]
public class CreateTablePayment : Migration
{
    public override void Down()
    {
        Delete.Table("Payments");
    }

    public override void Up()
    {
        Console.WriteLine("Creating table Payments.");

        if (!Schema.Table("Payments").Exists())
        {
            Create.Table("Payments")
                .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("OrderId").AsGuid().NotNullable()
                .WithColumn("CustomerId").AsGuid().NotNullable()
                .WithColumn("Amount").AsDecimal().NotNullable()
                .WithColumn("Method").AsInt32().NotNullable()
                .WithColumn("Status").AsInt32().NotNullable()
                .WithColumn("PaymentDate").AsDateTimeOffset().NotNullable()
                .WithColumn("TransactionId").AsString().NotNullable();
        }

        Console.WriteLine("Creating table Payments.");
    }

}