using Tesis.Domain.Models;

namespace Tesis.DataAcces.Repository.IRepository
{
    public interface IIndicadorRepository:IRepository<IndicadorModel>
    {
        void Update(IndicadorModel Indicador);
    }
}
