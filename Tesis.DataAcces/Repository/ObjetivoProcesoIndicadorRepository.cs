using System;
using System.Collections.Generic;
using System.Linq;
using Tesis.DataAcces.Repository.IRepository;
using Tesis.Domain.Models;

namespace Tesis.DataAcces.Repository
{
    public class ObjetivoProcesoIndicadorRepository : Repository<ObjetivoProcesoIndicadorModel>, IObjetivoProcesoIndicadorRepository
    {
        private readonly ApplicationDbContext _context;
        public ObjetivoProcesoIndicadorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ObjetivoProcesoIndicadorModel ObjetivoProcesoIndicador)
        {
            _context.Update(ObjetivoProcesoIndicador);
        }
    }
}
