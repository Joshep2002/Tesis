using System.Linq.Expressions;

namespace Tesis.DataAcces.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //Retornar todos los valores.
        Task<IEnumerable<T>> GetAll();

        //Retornar por id o cualquier otro parametro que cumpla
        Task<T> Get(Expression<Func<T, bool>> filter);

        //Retornar todos los valores por algun parametro que cumpla
        Task<IEnumerable<T>> GetAllValuesByParameter(Expression<Func<T, bool>> filter);

        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void AddRange(IEnumerable<T> entities);

        // Nuevo método para incluir propiedades de navegación
        Task<T> GetWithIncludes(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<IEnumerable<T>> GetAllWithIncludes(string? includeProperties = null);

        // Paginacion
        Task<IEnumerable<T>> GetAllPaged(int pageNumber, int pageSize,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

    }
}
