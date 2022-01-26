using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OperatorSharp.Extensions.DependencyInjection
{
    public class OperatorScope<TOperator> : IOperatorScope where TOperator : OperatorSharp.Operator
    {
        private bool disposedValue;

        private CancellationTokenSource cts;
        private IServiceScope scope;

        private Task watcherTask;
        private readonly IHostLifetime lifetime;
        private readonly IOptions<HostOptions> hostOptions;

        public OperatorScope(IServiceProvider provider, IHostLifetime lifetime, IOptions<HostOptions> hostOptions)
        {
            this.scope = provider.CreateScope();
            this.lifetime = lifetime;
            this.hostOptions = hostOptions;
        }

        public async Task StartAsync(string watchedNamespace, CancellationToken cancellationToken)
        {
            // Create CTS for stop control
            cts = new CancellationTokenSource();
            // Create scope for service
            var instance = scope.ServiceProvider.GetService<TOperator>();
            var logger = scope.ServiceProvider.GetService<ILogger<OperatorScope<TOperator>>>();

            try
            {
                watcherTask = instance.WatchAsync(cts.Token, watchedNamespace);
                watcherTask.ContinueWith(async task =>
                {
                    if (task.IsFaulted)
                    {
                        var source = new CancellationTokenSource();
                        source.CancelAfter((int)(hostOptions.Value.ShutdownTimeout.TotalMilliseconds));
                        await lifetime.StopAsync(source.Token);
                    }
                    else
                    {
                        await StartAsync(watchedNamespace, cts.Token);
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError("An error occurred during operator execution", ex);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop TCS
            cts.Cancel();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cts.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~OperatorScope()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
