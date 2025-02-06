﻿using Application.Dtos.Dtos.Payments;
using Billing.Domain.Entities;

namespace Billing.Service.Extentions;

public static class PaymentExtentions
{
    public static Payment ToPayment(this PaymentCreatDto payment)
    {
        return new Payment
        {
            OrderId = payment.OrderId,
            CustomerId = payment.CustomerId,
            CreditCard = new CreditCard
            {
                CardNumber = payment.CreditCard?.CardNumber,
                ExpirationDate = payment.CreditCard?.ExpirationDate,
                Holder = payment.CreditCard?.Holder,
                SecurityCode = payment.CreditCard?.SecurityCode
            }
        };
    }
}
