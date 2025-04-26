using Tesis.DataAcces.Repository.IRepository;

namespace Tesis.DataAcces.Repository
{
    public class UnitOfWorks : IUnitOfWorks
    {
        private readonly ApplicationDbContext _context;

        public IIndicadorRepository Indicador { get; private set; }
        public IProcesoRepository Proceso { get; private set; }
        public IObjetivoRepository Objetivo { get; private set; }

        public UnitOfWorks(ApplicationDbContext context)
        {
            _context = context;
            Indicador = new IndicadorRepository(_context);
            Proceso = new ProcesoRepository(_context);
            Objetivo = new ObjetivoRepository(_context);
        }

        public void Save()
        {
              _context.SaveChanges();
        }
        public async Task SaveAsync()
        {
          await _context.SaveChangesAsync();
        }
    }
}
