using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using prjEvolutionAPI.Repositories.Interfaces;
using prjEvolutionAPI.Models;
using System.Collections.Generic;

namespace prjEvolutionAPI.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly EvolutionApiContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(EvolutionApiContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void Remove(T entity)
            => _dbSet.Remove(entity);

        public void AddRange(IEnumerable<T> entity) =>
        _dbSet.AddRange(entity);
    }
}
