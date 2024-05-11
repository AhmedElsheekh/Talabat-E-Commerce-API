using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Spec;

namespace Talabat.Service
{
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration _configuration;
		private readonly IBasketRepository _basketRepository;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentService(IConfiguration configuration,
			IBasketRepository basketRepository,
			IUnitOfWork unitOfWork)
		{
			_configuration = configuration;
			_basketRepository = basketRepository;
			_unitOfWork = unitOfWork;
		}
		public async Task<CustomerBasket?> CreateOrUpdatePaymentIntentAsync(string basketId)
		{
			StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];

			//To create paymentIntent, I need basket total amount
			var basket = await _basketRepository.GetBasketAsync(basketId);
			if (basket is null) return null;

			//Get DeliveryMethod to calculate total
			decimal shippingPrice = 0M;
			if(basket.DeliveryMethodId.HasValue)
			{
				var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
				shippingPrice = deliveryMethod.Cost;
			}

			//Calculate subtotal of basket items
			if(basket.Items.Count > 0)
			{
				foreach(var item in basket.Items)
				{
					var product = await _unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.Id);
					if (item.Price != product.Price)
						item.Price = product.Price;
				}
			}

			var subTotal = basket.Items.Sum(item => item.Price * item.Quantity);

			//Now start to create paymentIntent
			var service = new PaymentIntentService();

			PaymentIntent paymentIntent;
			if(string.IsNullOrEmpty(basket.PaymentIntentId)) //Create
			{
				var options = new PaymentIntentCreateOptions()
				{
					Amount = (long)subTotal * 100 + (long)shippingPrice * 100,
					Currency = "usd",
					PaymentMethodTypes = new List<string>() { "card"}
				};

				paymentIntent = await service.CreateAsync(options);
				basket.PaymentIntentId = paymentIntent.Id;
				basket.ClientSecret = paymentIntent.ClientSecret;
			}
			else //Update
			{
				var options = new PaymentIntentUpdateOptions()
				{
					Amount = (long)subTotal * 100 + (long)shippingPrice * 100
				};

				paymentIntent = await service.UpdateAsync(basket.PaymentIntentId, options);
				basket.PaymentIntentId = paymentIntent.Id;
				basket.ClientSecret = paymentIntent.ClientSecret;
			}

			await _basketRepository.UpdateBasketAsync(basket);

			return basket;
		}

		public async Task<Order> UpdatePaymentIntentToSucceedOrFailed(string paymentIntentId, bool flag)
		{
			var spec = new OrderWithPaymentIntentSpec(paymentIntentId);
			var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

			if (flag)
				order.Status = OrderStatus.PaymentReceived;
			else
				order.Status = OrderStatus.PaymentFailed;

			_unitOfWork.Repository<Order>().Update(order);
			await _unitOfWork.CompleteAsync();

			return order;
		}
	}
}
