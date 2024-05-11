using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	public class OrdersController : APIBaseController
	{
		private readonly IOrderService _orderService;
		private readonly IMapper _mapper;

		public OrdersController(IOrderService orderService,
			IMapper mapper)
		{
			_orderService = orderService;
			_mapper = mapper;
		}

		[ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[Authorize]
		[HttpPost]
		public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var mappedAddress = _mapper.Map<Address>(orderDto.ShippingAddress);
			var order = await _orderService.CreateOrderAsync(buyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId,
				mappedAddress);
			if (order is null) return BadRequest(new ApiResponse(400, "There Is A Problem With Your Order"));
			return Ok(order);
		}


		[ProducesResponseType(typeof(IReadOnlyList<OrderToReturnDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpGet]
		[Authorize]
		public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var orders = await _orderService.GetAllOrdersForSpecificUserAsync(buyerEmail);
			if (orders.Count == 0) return NotFound(new ApiResponse(404, "There is no orders for this user"));
			var mappedOrders = _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
			return Ok(mappedOrders);
		}


		[ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[Authorize]
		[HttpGet("{orderId}")]
		public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int orderId)
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var order = await _orderService.GetOrderByIdForSpecificUserAsync(buyerEmail, orderId);
			if (order is null) return NotFound(new ApiResponse(404, $"There is no order with id = {orderId} for this user"));
			var mappedOrder = _mapper.Map<OrderToReturnDto>(order);
			return Ok(mappedOrder);
		}

		[HttpGet("DeliveryMethods")]
		public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethods()
			=> await _orderService.GetDeliveryMethodsAsync();
	}
}
