using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MO.Model.Context;
using System.Threading;
using System.Threading.Tasks;

namespace MO.Silo
{
    public class TaskService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly MODataContext _dataContext;
        private readonly MORecordContext _recordContext;
        private Timer _dataUpdateTimer;
        private Timer _recordUpdateTimer;

        public TaskService(
            ILogger<TaskService> logger,
            MODataContext dataContext,
            MORecordContext recordContext)
        {
            _logger = logger;
            _dataContext = dataContext;
            _recordContext = recordContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dataUpdateTimer = new Timer(OnDataTimerCallback, null, 100, 1000);
            _recordUpdateTimer = new Timer(OnRecordTimerCallback, null, 100, 5000);
            return Task.CompletedTask;
        }

        private void OnDataTimerCallback(object sender)
        {
            int count = _dataContext.SaveChanges();
            if (count != 0)
            {
                _logger.LogInformation("dataContext update {0}", count);
            }
        }

        private void OnRecordTimerCallback(object sender)
        {
            int count = _recordContext.SaveChanges();
            if (count != 0)
            {
                _logger.LogInformation("recordContext update {0}", count);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _dataUpdateTimer.Dispose();
            _recordUpdateTimer.Dispose();
            return Task.CompletedTask;
        }
    }
}
