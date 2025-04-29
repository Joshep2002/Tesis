using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis.Domain.Models;

namespace Tesis.DataAcces.Repository.IRepository
{
    public interface IObjetivoProcesoIndicadorRepository:IRepository<ObjetivoProcesoIndicadorModel>
    {
        void Update(ObjetivoProcesoIndicadorModel ObjetivoProcesoIndicador);
    }
}
