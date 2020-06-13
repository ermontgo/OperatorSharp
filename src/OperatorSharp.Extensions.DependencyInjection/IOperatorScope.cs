using System;
using System.Threading;
using System.Threading.Tasks;

namespace OperatorSharp.Extensions.DependencyInjection
{
    public interface IOperatorScope : IDisposable
    {
        Task StartAsync(string watchedNamespace, CancellationToken token);
    }
}
