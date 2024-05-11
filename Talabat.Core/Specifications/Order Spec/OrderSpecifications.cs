using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.Order_Spec
{
	public class OrderSpecifications : BaseSpecifications<Order>
	{
		public OrderSpecifications(string buyerEmail) : base(O => O.BuyerEmail == buyerEmail)
		{
			Includes.Add(O => O.DeliveryMethod);
			Includes.Add(O => O.Items);

			AddOrderByDescending(O => O.OrderDate);
		}

		public OrderSpecifications(string buyerEmail, int orderId) : base(O => O.BuyerEmail == buyerEmail && O.Id == orderId)
		{
			Includes.Add(O => O.DeliveryMethod);
			Includes.Add(O => O.Items);
		}
	}
}
