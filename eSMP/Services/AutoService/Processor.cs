using eSMP.Models;
using eSMP.Services.OrderRepo;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace eSMP.Services.AutoService
{
    public class Processor : BackgroundService
    {
        private readonly IWorker _worker;

        public Processor(IWorker worker)
        {
            _worker = worker;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            while (stoppingToken.IsCancellationRequested == false)
            {
                //1000*60*15
                /*await Task.Delay(5000, stoppingToken);
                var role = _context.Roles.SingleOrDefault(r => r.RoleID == 5);
                role.RoleName = DateTime.Now.ToString();
                _context.SaveChanges();
                ControlOfRevenues()*/;
                await _worker.DoWork(stoppingToken);
            }
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _worker.DoWork(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
