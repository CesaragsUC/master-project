using AutoMapper;
using AutoMapper.QueryableExtensions;
using Billing.Application.Abstractions;
using Billing.Application.Dtos;
using Billing.Application.Extentions;
using Billing.Domain.Entities;
using Billing.Infrastructure;
using Billing.Infrastructure.Configurations.RabbitMq;
using HybridRepoNet.Abstractions;
using Message.Broker.Abstractions;
using Messaging.Contracts.Events.Payments;
using Microsoft.EntityFrameworkCore;
using ResultNet;
using Serilog;
using Shared.Kernel.Core.Enuns;
using Shared.Kernel.Utils;

namespace Billing.Application.Service;


public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork<BillingContext> _unitOfWork;
    private readonly IQueueService _queueService;
    private readonly IOderApi _oderApi;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IMapper _mapper;

    public PaymentService(
       IUnitOfWork<BillingContext> unitOfWork,
       IQueueService queueService,
        IOderApi oderApi,
        IRabbitMqService rabbitMqService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _queueService = queueService;
        _oderApi = oderApi;
        _rabbitMqService = rabbitMqService;
        _mapper = mapper;
    }

    public async Task<Result<bool>> CreatePaymentAsync(PaymentCreatDto paymentDto)
    {
        try
        {
            var orderResult = await GetOrderAsync(paymentDto.OrderId, paymentDto.CustomerId);
            if (!orderResult.Succeeded)
            {
                return await Result<bool>.FailureAsync(orderResult.Messages!);
            }

            var payment = CreatePaymentEntity(paymentDto, orderResult!.Data!.ToOrder()!);

            await _unitOfWork.Repository<Payment>().AddAsync(payment);
            await _unitOfWork.Commit();

            var paymentCreated = CreatePaymentCreatedEvent(payment, orderResult!.Data!.ToOrder()!);

            await _rabbitMqService.Send(paymentCreated, _queueService.PaymentCreatedMessage);

            return await Result<bool>.SuccessAsync("Payment created");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error on create payment");
            throw;
        }
    }

    private async Task<Result<OrderDto>> GetOrderAsync(Guid orderId, Guid customerId)
    {
        var orderResponse = await _oderApi.GetOrderAsync(orderId, customerId);
        if (!orderResponse.IsSuccessStatusCode)
        {
            return await Result<OrderDto>.FailureAsync("Order not founded");
        }
        return await Result<OrderDto>.SuccessAsync(orderResponse.Content!.Data!);
    }

    private Payment CreatePaymentEntity(PaymentCreatDto paymentDto, Order order)
    {
        var payment = paymentDto.ToPayment();
        payment.Amount = order.TotalAmount;
        payment.Status = (int)PaymentStatus.Pending;
        payment.Method = (int)PaymentMethod.CreditCard;
        payment.PaymentDate = DateTime.Now;
        payment.TransactionId = PaymentHelper.GenerateTransactionIdWithPrefix();
        return payment;
    }

    private PaymentCreatedEvent CreatePaymentCreatedEvent(Payment payment, Order order)
    {
        return new PaymentCreatedEvent
        {
            OrderId = order.Id,
            PaymentToken = PaymentTokenService.GenerateToken(),
            Amount = order.TotalAmount,
            CustomerId = order.CustomerId,
            PaymentDate = DateTime.Now,
            Status = (int)PaymentStatus.Pending,
            PaymentMethod = (int)PaymentMethod.CreditCard,
            CreditCard = new Messaging.Contracts.Events.Payments.CreditCard
            {
                CardNumber = payment.CreditCard!.CardNumber,
                ExpirationDate = payment.CreditCard.ExpirationDate,
                SecurityCode = payment.CreditCard.SecurityCode,
                Holder = payment.CreditCard.Holder,
            }
        };
    }



    public async Task<Result<bool>> DeletePaymentAsync(string transactionId)
    {
        try
        {
            await _unitOfWork.Repository<Payment>().SoftDeleteAsync(x => x.TransactionId == transactionId);
            await _unitOfWork.Commit();

            return await Result<bool>.SuccessAsync("Payment deleted");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error on delete payment");
            throw;
        }

    }

    public async Task<Result<IEnumerable<PaymentDto>>> GetAllPaymentAsync()
    {
        var result = await _unitOfWork.Repository<Payment>().GetAllAsync();

       var listDto = _mapper.Map<IEnumerable<PaymentDto>>(result.Where(x => !x.IsDeleted).ToList());

        return await Result<IEnumerable<PaymentDto>>.SuccessAsync(listDto);
    }

    public async Task<Result<PaymentDto>> GetPaymentAsync(string transactionId)
    {
        var result = await _unitOfWork.Repository<Payment>().FindAsync(x => x.TransactionId == transactionId);

        var paymentDto = _mapper.Map<PaymentDto>(result);

        return await Result<PaymentDto>.SuccessAsync(paymentDto);
    }
}
