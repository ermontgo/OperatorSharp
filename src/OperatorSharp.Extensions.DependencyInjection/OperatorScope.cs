using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OperatorSharp.Extensions.DependencyInjection
{
    public class OperatorScope<TOperator> : IOperatorScope where TOperator : OperatorSharp.Operator
    {
        private readonly IServiceScope scope;
        public OperatorScope(IServiceProvider provider)
        {
            this.scope = provider.CreateScope();
        }

        public async Task StartAsync(string watchedNamespace, CancellationToken token)
        {
            var instance = scope.ServiceProvider.GetService<TOperator>();
            var logger = scope.ServiceProvider.GetService<ILogger<OperatorScope<TOperator>>>();

            try
            {
                await instance.WatchAsync(token, watchedNamespace);
            }
            catch (Exception ex)
            {
                logger.LogError("An error occurred during operator execution", ex);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    scope.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}
