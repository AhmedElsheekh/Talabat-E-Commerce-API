using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public class ProductWithBrandAndTypeSpecifications : BaseSpecifications<Product>
	{
		// Get All Products
		public ProductWithBrandAndTypeSpecifications(ProductSpecificationsParameters parameters) 
			: base(P => 
			      (string.IsNullOrEmpty(parameters.Search) || P.Name.ToLower().Contains(parameters.Search))
			      &&
			      (!parameters.TypeId.HasValue || P.ProductTypeId == parameters.TypeId)
			      &&
			      (!parameters.BrandId.HasValue || P.ProductBrandId == parameters.BrandId)
				  )
		{
			Includes.Add(P => P.ProductBrand);
			Includes.Add(P => P.ProductType);

			if(!string.IsNullOrEmpty(parameters.Sort))
			{
				switch(parameters.Sort)
				{
					case "PriceAsc":
						AddOrderBy(P => P.Price);
						break;
					case "PriceDesc":
						AddOrderByDescending(P => P.Price);
						break;
					default:
						AddOrderBy(P => P.Name);
						break;
				}
			}

			ApplyPagination(parameters.PageSize * (parameters.PageIndex - 1), parameters.PageSize);
		}

		//Get Product By Id
		public ProductWithBrandAndTypeSpecifications(int id) : base(P => P.Id == id)
		{

			Includes.Add(P => P.ProductBrand);
			Includes.Add(P => P.ProductType);
		}
	}
}
