using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public interface ISpecifications<T> where T : BaseEntity
	{
		//_context.Products.where(P => P.ProductId == id).include(P => P.ProductType).include(P => P.ProductBrand)
		Expression<Func<T, bool>> Criteria { get; set; }
		List<Expression<Func<T, object>>> Includes { get; set; }
		Expression<Func<T, object>> OrderBy { get; set; }
		Expression<Func<T, object>> OrderByDescending { get; set; }
		int Take { get; set; }
		int Skip { get; set; }
		bool IsPaginationEnabled { get; set; }
	}
}
