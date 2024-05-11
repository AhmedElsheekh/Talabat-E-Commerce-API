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
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepos;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPaymentService _paymentService;

		public OrderService(
			IBasketRepository basketRepos,
			IUnitOfWork unitOfWork,
			IPaymentService paymentService)
		{
			_basketRepos = basketRepos;
			_unitOfWork = unitOfWork;
			_paymentService = paymentService;
		}

		public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
		{
			//1- Get DeliveryMethod
			var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

			//2- Get ICollection<OrderItem> Items
			//2.1 Get Basket
			var basket = await _basketRepos.GetBasketAsync(basketId);

			//2.2 Get Products From Database By Using Basket Items, Then Compose Icollection<OrderItem> orderItems
			var orderItems = new List<OrderItem>();
			if(basket?.Items.Count > 0)
			{
				foreach(var item in basket.Items)
				{
					var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
					var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
					var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);
					orderItems.Add(orderItem);
				}
			}

			//3- Get SubTotal
			var subTotal = orderItems.Sum(O => O.Quantity * O.Price);

			//4- Create Order
			var spec = new OrderWithPaymentIntentSpec(basket.PaymentIntentId);
			var exOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

			if(exOrder is not null)
			{
				_unitOfWork.Repository<Order>().Delete(exOrder);
				await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId);
			}

			var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, subTotal, basket.PaymentIntentId);

			//5- Add Order Locally
			await _unitOfWork.Repository<Order>().AddAsync(order);

			//6- Save To Database
			var result = await _unitOfWork.CompleteAsync();

			//7- Return Order
			if (result <= 0) return null;
			return order;

		}

		public async Task<IReadOnlyList<Order>> GetAllOrdersForSpecificUserAsync(string buyerEmail)
		{
			var spec = new OrderSpecifications(buyerEmail);
			var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
			return orders;
		}

		public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
			=> await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

		public async Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId)
		{
			var spec = new OrderSpecifications(buyerEmail, orderId);
			var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
			return order;
		}
	}
}
