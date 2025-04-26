namespace Tesis.DataAcces.Repository.IRepository
{
    public interface IUnitOfWorks
    {
        IIndicadorRepository Indicador { get;}
        IProcesoRepository Proceso { get;}
        IObjetivoRepository Objetivo { get;}

        Task SaveAsync();
        void Save();
    }
}
