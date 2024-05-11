using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public class ProductsWithFilterationForCount : BaseSpecifications<Product>
	{
		public ProductsWithFilterationForCount(ProductSpecificationsParameters parameters)
			: base(P =>
				(string.IsNullOrEmpty(parameters.Search) || P.Name.ToLower().Contains(parameters.Search))
				&&
				(!parameters.TypeId.HasValue || P.ProductTypeId == parameters.TypeId)
				&&
				(!parameters.BrandId.HasValue || P.ProductBrandId == parameters.BrandId)
				)
		{

		}
	}
}
