using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	public class PaymentsController : APIBaseController
	{
		private readonly IPaymentService _paymentService;
		private readonly IMapper _mapper;
		private const string endpointSecret = "whsec_0da821eb893030855b184e01854b43efd2c8a0f5a59a1445139961413f2e640d";

		public PaymentsController(IPaymentService paymentService,
			IMapper mapper)
		{
			_paymentService = paymentService;
			_mapper = mapper;
		}

		[ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		[Authorize]
		[HttpPost]
		public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
		{
			var customerBasket = await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId);
			if (customerBasket is null) return BadRequest(new ApiResponse(400, "Error with your basket"));
			var mappedCustomerBasket = _mapper.Map<CustomerBasketDto>(customerBasket);
			return Ok(mappedCustomerBasket);
		}

		[HttpPost("webhook")]
		public async Task<IActionResult> StripeWebHook()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			try
			{
				var stripeEvent = EventUtility.ConstructEvent(json,
					Request.Headers["Stripe-Signature"], endpointSecret);

				var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

				// Handle the event
				if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
				{
					await _paymentService.UpdatePaymentIntentToSucceedOrFailed(paymentIntent.Id, false);
				}
				else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
				{
					await _paymentService.UpdatePaymentIntentToSucceedOrFailed(paymentIntent.Id, true);
				}

				return Ok();
			}
			catch (StripeException e)
			{
				return BadRequest();
			}
		}
	}
}
