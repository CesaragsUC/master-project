using Order.Application.Abstractions;
using Order.Application.Dto;
using Order.Application.Extentions;
using Order.Domain.Abstraction;
using ResultNet;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Order.Application.Service;

[ExcludeFromCodeCoverage]
public class OrderService : IOrderService
{
    private readonly IOrderRepository _unitOfWork;

    public OrderService(IOrderRepository unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Add(CreateOrderDto orderDto)
    {
        try
        {
            await _unitOfWork.AddAsync(orderDto.ToOrder());
            await _unitOfWork.Commit();

            return await Result<bool>.SuccessAsync(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while adding order");
            throw;
        }

    }

    public async Task<Result<bool>> Delete(Guid id)
    {
        try
        {
            var entity = await _unitOfWork.FindAsync(x => x.Id == id);
            if (entity is null)
            {
                return await Result<bool>.FailureAsync("Order not found");
            }

            _unitOfWork.SoftDelete(entity);
            await _unitOfWork.Commit();

            return await Result<bool>.SuccessAsync(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex,"Error while deleting order");
            throw;
        }

    }

    public async Task<Result<IEnumerable<OrderDto>>> Get(Guid custumerId)
    {
        try
        {
            var order = await _unitOfWork.GetAllAsync(x => x.CustomerId == custumerId, o => o.Items!);

            if (order is null)
            {
                return await Result<IEnumerable<OrderDto>>.FailureAsync("Order not found");
            }

            var orderDto = order.ToList();
            return await Result<IEnumerable<OrderDto>>.SuccessAsync(orderDto.Select(x => x.ToOrderDto()));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while getting order");
            throw;
        }
    }


    public async Task<Result<OrderDto>> Get(Guid orderId,Guid customerId)
    {
        try
        {
            var order = await _unitOfWork.FindAsync(
                x => x.Id == orderId 
                && x.CustomerId == customerId,
                i => i.Items!);

            if (order is null)
            {
                return await Result<OrderDto>.FailureAsync("Order not found");
            }

            return await Result<OrderDto>.SuccessAsync(order.ToOrderDto());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while getting order");
            throw;
        }

    }

    public async Task<Result<IEnumerable<OrderDto>>> List()
    {
        try
        {
            var list = await _unitOfWork.GetAllAsync(1, 2, x => x.Items!);

            if (list is null)
            {
                return await Result<IEnumerable<OrderDto>>.FailureAsync("Orders not found");
            }

            return await Result<IEnumerable<OrderDto>>.SuccessAsync(list.Select(x => x.ToOrderDto()));

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while listing orders");
            throw;
        }

    }

    public async Task<Result<bool>> Update(UpdateOrderDto orderDto)
    {
        try
        {
            var order = await _unitOfWork.FindAsync(x => x.Id == orderDto.Id);

            if (order is null)
            {
                return await Result<bool>.FailureAsync("Order not found");
            }

            order.CustomerId = orderDto.CustomerId;
            order.Items = orderDto?.Items?.Select(x => x.ToOrderItem(order.Id));
            order.Status = orderDto!.Status;

            _unitOfWork.Update(order);

            return await Result<bool>.SuccessAsync(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while updating order");
            throw;
        }

    }
}
