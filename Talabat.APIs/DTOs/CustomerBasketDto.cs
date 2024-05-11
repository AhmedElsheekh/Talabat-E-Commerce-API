﻿namespace Talabat.APIs.DTOs
{
	public class CustomerBasketDto
	{
		public string Id { get; set; }
		public string? PaymentIntentId { get; set; }
		public string? ClientSecret { get; set; }
		public int? DeliveryMethodId { get; set; }
		public List<BasketItemDto> Items { get; set; }
	}
}
