using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Talabat.Core;

namespace Talabat.APIs.Controllers
{
	public class ProductsController : APIBaseController
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ProductsController(IUnitOfWork unitOfWork,
			IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		[Authorize]
		[HttpGet]
		public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecificationsParameters parameters)
		{
			var spec = new ProductWithBrandAndTypeSpecifications(parameters);

			var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
			//OkObjectResult result = new OkObjectResult(products);
			//return result;
			var mappedProducts = _mapper.Map<IReadOnlyList<ProductToReturnDto>>(products);
			var countSpec = new ProductsWithFilterationForCount(parameters);
			var count = await _unitOfWork.Repository<Product>().GetEntitiesCountWithSpecAsync(countSpec);
			return Ok(new Pagination<ProductToReturnDto>(parameters.PageSize, parameters.PageIndex, mappedProducts, count));
		}

		[HttpGet("{id}")]
		[ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		public async Task<ActionResult<ProductToReturnDto>> GetProductById (int id)
		{
			var spec = new ProductWithBrandAndTypeSpecifications(id);

			var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);
			if (product is null) return NotFound(new ApiResponse(404));
			var mappedProduct = _mapper.Map<ProductToReturnDto>(product);
			return Ok(mappedProduct);
		}

		[HttpGet("Types")]
		public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
		{
			var types = await _unitOfWork.Repository<ProductType>().GetAllAsync();
			return Ok(types);
		}

		[HttpGet("Brands")]
		public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
		{
			var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
			return Ok(brands);
		}

	}
}
