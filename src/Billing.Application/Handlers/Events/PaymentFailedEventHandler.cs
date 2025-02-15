﻿using AutoMapper;
using MediatR;
using Messaging.Contracts.Events.Payments;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Application.Handlers.Events;

[ExcludeFromCodeCoverage]
public class PaymentFailedEventHandler :
    IRequestHandler<PaymentFailedEvent, bool>
{
    private readonly IMapper _mapper;

    public PaymentFailedEventHandler(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<bool> Handle(PaymentFailedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fail to create a order");
            return false;
        }

    }
}