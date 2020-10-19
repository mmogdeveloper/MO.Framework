using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MO.Model.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MO.Silo
{
    public class DBTaskService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly MODataContext _dataContext;
        private readonly MORecordContext _recordContext;
        private Timer _dataUpdateTimer;
        private Timer _recordUpdateTimer;
        private int _isDataUpdateRunning;
        private int _isRecordUpdateRunning;

        public DBTaskService(
            ILogger<DBTaskService> logger,
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
            if (Interlocked.CompareExchange(ref _isDataUpdateRunning, 1, 0) == 0)
            {
                try
                {
                    int count = _dataContext.SaveChanges();
                    if (count != 0)
                    {
                        _logger.LogInformation("dataContext update {0}", count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("{0},{1}", ex.Message, ex.StackTrace);
                }
                Interlocked.Exchange(ref _isDataUpdateRunning, 0);
            }
        }

        private void OnRecordTimerCallback(object sender)
        {
            if (Interlocked.CompareExchange(ref _isRecordUpdateRunning, 1, 0) == 0)
            {
                try
                {
                    int count = _recordContext.SaveChanges();
                    if (count != 0)
                    {
                        _logger.LogInformation("recordContext update {0}", count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("{0},{1}", ex.Message, ex.StackTrace);
                }
                Interlocked.Exchange(ref _isRecordUpdateRunning, 0);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            OnDataTimerCallback(null);
            OnRecordTimerCallback(null);
            _dataUpdateTimer.Dispose();
            _recordUpdateTimer.Dispose();
            return Task.CompletedTask;
        }
    }
}
