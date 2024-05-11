using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
	{
		private readonly StoreDbContext _context;

		public GenericRepository(StoreDbContext context)
		{
			_context = context;
		}

		#region Without Specifications
		public async Task<IReadOnlyList<T>> GetAllAsync()
		{
			//if (typeof(T) == typeof(Product))
			//	return (IReadOnlyList<T>)await _context.Products.Include(p => p.ProductBrand).Include(p => p.ProductType).ToListAsync();

			return await _context.Set<T>().ToListAsync();
		}

	
		public async Task<T> GetByIdAsync(int id)
			=> await _context.Set<T>().FindAsync(id);


		#endregion

		#region With Specifications
		public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
			=> await ApplySpecifications(spec).ToListAsync();

		public async Task<T> GetEntityWithSpecAsync(ISpecifications<T> spec)
			=> await ApplySpecifications(spec).FirstOrDefaultAsync();

		private IQueryable<T> ApplySpecifications(ISpecifications<T> spec)
			=> SpecificationsEvaluator<T>.GetQuery(_context.Set<T>(), spec);

		public async Task<int> GetEntitiesCountWithSpecAsync(ISpecifications<T> spec)
			=> await ApplySpecifications(spec).CountAsync();

		public async Task AddAsync(T entity)
			=> await _context.Set<T>().AddAsync(entity);

		public void Delete(T entity)
			=> _context.Set<T>().Remove(entity);

		public void Update(T entity)
			=> _context.Set<T>().Update(entity);

		#endregion
	}
}
