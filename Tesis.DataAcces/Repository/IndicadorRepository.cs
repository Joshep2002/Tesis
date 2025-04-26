using Tesis.DataAcces.Repository.IRepository;
using Tesis.Domain.Models;


namespace Tesis.DataAcces.Repository
{
    public class IndicadorRepository : Repository<IndicadorModel>, IIndicadorRepository
    {
        private readonly ApplicationDbContext _context;

        public IndicadorRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public void Update(IndicadorModel Indicador)
        {
         
             _context.Update(Indicador);
        }

    }

}
