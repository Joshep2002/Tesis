using Tesis.Domain.Models;

namespace Tesis.DataAcces.Repository.IRepository
{ 
    public interface IProcesoRepository : IRepository<ProcesoModel>
    {
        void Update(ProcesoModel Proceso);
    }
}
