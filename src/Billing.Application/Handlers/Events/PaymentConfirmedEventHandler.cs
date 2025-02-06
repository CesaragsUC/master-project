using AutoMapper;
using MediatR;
using Messaging.Contracts.Events.Payments;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Application.Handlers.Events;

[ExcludeFromCodeCoverage]
public class PaymentConfirmedEventHandler :
    IRequestHandler<PaymentConfirmedEvent, bool>
{

    private readonly IMapper _mapper;

    public PaymentConfirmedEventHandler(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<bool> Handle(PaymentConfirmedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            //var order = _mapper.Map<Models.Order>(request);


            //Log.Information("Order created: {Id} - {Date}", order.Id, DateTime.Now);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fail to create a order");
            return false;
        }

    }
}
