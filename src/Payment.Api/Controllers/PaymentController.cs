using Application.Dtos.Dtos.Payments;
using Billing.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Api.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/payment")]
public class PaymentController(IPaymentService paymentService) : ControllerBase
{
    [HttpPost]
    [Route("process")]
    public async Task<IActionResult> ProcessPayment(PaymentCreatDto paymentDto)
    {
        var result = await paymentService.CreatePaymentAsync(paymentDto);
        return result.Succeeded ? Ok() : BadRequest(result);
    }

    [HttpGet]
    [Route("payments")]
    public async Task<IActionResult> ListAsync()
    {
        var result = await paymentService.GetAllPaymentAsync();
        return result.Succeeded ? Ok() : BadRequest(result);
    }


    [HttpGet]
    [Route("delete/{transactionId}")]
    public async Task<IActionResult> GetByIdAsync(string transactionId)
    {
        var result = await paymentService.GetPaymentAsync(transactionId);
        return result.Succeeded ? Ok() : BadRequest(result);
    }

    [HttpDelete]
    [Route("delete/{transactionId}")]
    public async Task<IActionResult> DeleteAsync(string transactionId)
    {
        var result = await paymentService.DeletePaymentAsync(transactionId);
        return result.Succeeded ? Ok() : BadRequest(result);
    }
}
