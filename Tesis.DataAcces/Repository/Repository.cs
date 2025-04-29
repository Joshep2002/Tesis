using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;
using Tesis.DataAcces.Repository.IRepository;

namespace Tesis.DataAcces.Repository
{

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
            _context.Add(entity);

        }

        public async Task<T> Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;

            query = query.Where(filter);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            IQueryable<T> query = dbSet;

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllValuesByParameter(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;

            query = query.Where(filter);

            return await query.ToListAsync();
        }

        public void Remove(T entity)
        {
            _context.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.RemoveRange(entities);
        }


        public async Task<IEnumerable<T>> GetAllPaged(int pageNumber, int pageSize,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return await query.Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();
        }


        // Implementación del nuevo método para incluir propiedades
        public async Task<T> GetWithIncludes(Expression<Func<T, bool>> filter,string? includeProperties)
        {
            IQueryable<T> query = dbSet.AsSplitQuery();

            // Aplica el filtro
            query = query.Where(filter);

            // Aplica propiedades relacionadas
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
                ;
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllWithIncludes(string? includeProperties)
        {
            IQueryable<T> query = dbSet.AsSplitQuery();

            // Aplica propiedades relacionadas
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
                  ;
            }

            return await query.ToListAsync();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _context.AddRange(entities);
        }
    }
}
