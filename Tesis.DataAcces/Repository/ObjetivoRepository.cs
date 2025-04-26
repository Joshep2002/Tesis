using Tesis.DataAcces.Repository.IRepository;
using Tesis.Domain.Models;

namespace Tesis.DataAcces.Repository
{
    public class ObjetivoRepository : Repository<ObjetivoModel>, IObjetivoRepository
    {
        private readonly ApplicationDbContext _context;

        public ObjetivoRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ObjetivoModel Objetivo)
        {
            _context.Update(Objetivo);
        }

    }
}
