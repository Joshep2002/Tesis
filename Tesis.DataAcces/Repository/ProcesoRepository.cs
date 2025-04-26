using Tesis.DataAcces.Repository.IRepository;
using Tesis.Domain.Models;

namespace Tesis.DataAcces.Repository
{
    public class ProcesoRepository : Repository<ProcesoModel>, IProcesoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProcesoRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ProcesoModel Proceso)
        {
            _context.Update(Proceso);
        }

    }
}
