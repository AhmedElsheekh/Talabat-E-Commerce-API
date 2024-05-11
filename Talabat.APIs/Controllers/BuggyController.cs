using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
	public class BuggyController : APIBaseController
	{
		private readonly StoreDbContext _context;

		public BuggyController(StoreDbContext context)
		{
			_context = context;
		}

		[HttpGet("NotFound")]
		public ActionResult GetNotFound()
		{
			var product = _context.Products.Find(100);

			if (product is null) return NotFound(new ApiResponse(404));
			return Ok(product);
		}

		[HttpGet("ServerError")]
		public ActionResult GetServerError()
		{
			var product = _context.Products.Find(100);

			var result = product.ToString();

			return Ok(result);
		}

		[HttpGet("ValidationError/{id}")]
		public ActionResult GetValidation(int id)
		{
			return Ok();
		}

		[HttpGet("BadRequest")]
		public ActionResult GetBadRequest()
		{
			return BadRequest(new ApiResponse(400));
		}

	}
}
