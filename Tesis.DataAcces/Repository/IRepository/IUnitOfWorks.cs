namespace Tesis.DataAcces.Repository.IRepository
{
    public interface IUnitOfWorks
    {
        IIndicadorRepository Indicador { get;}
        IProcesoRepository Proceso { get;}
        IObjetivoRepository Objetivo { get;}
        IObjetivoProcesoIndicadorRepository ObjetivoProcesoIndicador { get; }
        IUserRepository Usuario { get; }
        Task SaveAsync();
        void Save();
    }
}
