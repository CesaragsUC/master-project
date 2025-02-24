using Billing.Application.Dtos;
using Billing.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Api.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/billing")]
[Authorize]
public class PaymentController(IPaymentService paymentService) : ControllerBase
{
    [HttpPost]
    [Route("process")]
    public async Task<IActionResult> ProcessPayment(PaymentCreatDto paymentDto)
    {
        var result = await paymentService.CreatePaymentAsync(paymentDto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    [Route("payments")]
    [Authorize(Roles = "Read")]
    public async Task<IActionResult> ListAsync()
    {
        var result = await paymentService.GetAllPaymentAsync();
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }


    [HttpGet]
    [Route("{transactionId}")]
    [Authorize(Roles = "Read")]
    public async Task<IActionResult> GetByIdAsync(string transactionId)
    {
        var result = await paymentService.GetPaymentAsync(transactionId);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete]
    [Route("delete/{transactionId}")]
    [Authorize(Roles = "Delete")]
    public async Task<IActionResult> DeleteAsync(string transactionId)
    {
        var result = await paymentService.DeletePaymentAsync(transactionId);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
