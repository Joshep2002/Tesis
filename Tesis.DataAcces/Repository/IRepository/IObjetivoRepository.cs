 using Tesis.Domain.Models;

namespace Tesis.DataAcces.Repository.IRepository
{
    public interface IObjetivoRepository : IRepository<ObjetivoModel>
    {
        void Update(ObjetivoModel Objetivo);
    }
}
