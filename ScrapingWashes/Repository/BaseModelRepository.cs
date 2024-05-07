using Microsoft.EntityFrameworkCore;
using ScrapingWashes.Context;
using System.Linq.Expressions;

namespace ScrapingWashes.Repository
{
    public class BaseModelRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        public BaseModelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddOrUpdateAsync(T entity, Expression<Func<T, bool>> where)
        {
            var existingEntity = await _context.Set<T>().FirstOrDefaultAsync(where);
            if (existingEntity == null)
            {
                _context.Set<T>().Add(entity);
                await _context.SaveChangesAsync();

                return entity;
            }
            else
            {
                var properties = typeof(T).GetProperties().Where(p => p.Name != $"{typeof(T).Name}Id" && !p.Name.EndsWith("Id"));

                foreach (var property in properties)
                {
                    var value = property.GetValue(entity);

                    if (value == null || value == string.Empty)
                    {
                        continue;
                    }
                    property.SetValue(existingEntity, value);
                }
                await _context.SaveChangesAsync();
                return existingEntity;
            }
        }
    }
}
