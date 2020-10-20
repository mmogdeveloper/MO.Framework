using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MO.Model.Context;
using Newtonsoft.Json;
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

            _dataContext.ChangeTracker.Tracked += ChangeTracker_Tracked;
            _dataContext.ChangeTracker.StateChanged += ChangeTracker_StateChanged;
            _recordContext.ChangeTracker.Tracked += ChangeTracker_Tracked;
            _recordContext.ChangeTracker.StateChanged += ChangeTracker_StateChanged;

            //初始化数据库
            _dataContext.Database.Migrate();
            _recordContext.Database.Migrate();
        }

        private void ChangeTracker_StateChanged(object sender, Microsoft.EntityFrameworkCore.ChangeTracking.EntityStateChangedEventArgs e)
        {
            _logger.LogInformation("ChangeTracker_StateChanged:{0},{1},{2},{3},{4}", e.Entry.Entity.GetType().Name, e.OldState, e.NewState, e.Entry.State, JsonConvert.SerializeObject(e.Entry.Entity));
        }

        private void ChangeTracker_Tracked(object sender, Microsoft.EntityFrameworkCore.ChangeTracking.EntityTrackedEventArgs e)
        {
            _logger.LogInformation("ChangeTracker_Tracked:{0},{1},{3}", e.Entry.Entity.GetType().Name, e.Entry.State, JsonConvert.SerializeObject(e.Entry.Entity));
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
                    if (_dataContext.ChangeTracker.HasChanges())
                    {
                        int count = _dataContext.SaveChanges();
                        if (count != 0)
                        {
                            _logger.LogInformation("dataContext SaveChanges {0}", count);
                        }
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
                    if (_recordContext.ChangeTracker.HasChanges())
                    {
                        int count = _recordContext.SaveChanges();
                        if (count != 0)
                        {
                            _logger.LogInformation("recordContext SaveChanges {0}", count);
                        }
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
