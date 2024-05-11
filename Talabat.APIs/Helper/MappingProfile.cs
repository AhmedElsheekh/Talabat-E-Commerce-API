using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.APIs.Helper
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Product, ProductToReturnDto>()
				.ForMember(d => d.ProductBrand, O => O.MapFrom(s => s.ProductBrand.Name))
				.ForMember(d => d.ProductType, O => O.MapFrom(s => s.ProductType.Name))
				.ForMember(d => d.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());

			CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();

			CreateMap<AddressDto, Core.Entities.Order_Aggregate.Address>();

			CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();

			CreateMap<BasketItemDto, BasketItem>().ReverseMap();

			CreateMap<Order, OrderToReturnDto>()
				.ForMember(d => d.DeliveryMethod, O => O.MapFrom(S => S.DeliveryMethod.ShortName))
				.ForMember(d => d.DeliveryMethodCost, O => O.MapFrom(S => S.DeliveryMethod.Cost));

			CreateMap<OrderItem, OrderItemDto>()
				.ForMember(d => d.ProductId, O => O.MapFrom(S => S.Product.ProductId))
				.ForMember(d => d.ProductName, O => O.MapFrom(S => S.Product.ProductName))
				.ForMember(d => d.PictureUrl, O => O.MapFrom(S => S.Product.PictureUrl))
				.ForMember(d => d.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());
		}
	}
}
