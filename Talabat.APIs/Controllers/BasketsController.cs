using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{
	public class BasketsController : APIBaseController
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IMapper _mapper;

		public BasketsController(IBasketRepository basketRepository,
			IMapper mapper)
		{
			_basketRepository = basketRepository;
			_mapper = mapper;
		}

		// Get Or Recreate
		[HttpGet]
		public async Task<ActionResult<CustomerBasket>> GetBasket(string basketId)
		{
			var basket = await _basketRepository.GetBasketAsync(basketId);
			return basket is null ? new CustomerBasket(basketId) : basket;
		}

		// Update Or Create New 
		[HttpPost]
		public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
		{
			var mappedBasket = _mapper.Map<CustomerBasket>(basket);
			var updatedOrCreatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);
			if (updatedOrCreatedBasket is null) return BadRequest(new ApiResponse(400));
			return Ok(updatedOrCreatedBasket);
		}

		//Delete
		[HttpDelete]
		public async Task<ActionResult<bool>> DeleteBasket(string basketId)
		{
			return await _basketRepository.DeleteBasketAsync(basketId);
		}
	}
}
