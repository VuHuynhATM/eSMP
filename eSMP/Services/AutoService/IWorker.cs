namespace eSMP.Services.AutoService
{
    public interface IWorker
    {
        Task DoWork(CancellationToken cancellationToken);
    }
}
