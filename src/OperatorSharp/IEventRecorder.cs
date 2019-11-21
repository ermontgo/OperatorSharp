using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;

namespace OperatorSharp
{
    public interface IEventRecorder<TObject> where TObject : KubernetesObject
    {
        IKubernetes Kubernetes { get; }
        ILogger<EventRecorder<TObject>> Logger { get; }

        void Record(string action, string reason, string message, V1ObjectReference objRef);
    }
}
