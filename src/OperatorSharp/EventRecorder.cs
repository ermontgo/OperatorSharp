using System;
using System.Collections.Generic;
using System.Text;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;

namespace OperatorSharp
{
    public class EventRecorder<TObject> : IEventRecorder<TObject> where TObject : KubernetesObject
    {
        public EventRecorder(ILogger<EventRecorder<TObject>> logger, IKubernetes kubernetes)
        {
            Logger = logger;
            Kubernetes = kubernetes;
        }

        public ILogger<EventRecorder<TObject>> Logger { get; }
        public IKubernetes Kubernetes { get; }

        private Dictionary<EventKey, EventDetails> events = new Dictionary<EventKey, EventDetails>();

        public void Record(string action, string reason, string message, V1ObjectReference objRef)
        {
            var occurrence = DateTime.Now;
            var key = new EventKey($"{objRef.ApiVersion}/{objRef.Kind}", action, reason, message, $"{objRef.NamespaceProperty}/{objRef.Name}");
            if (!events.ContainsKey(key))
            {
                Logger.LogDebug("Initializing new EventDetail entry for {objName}", key.ToString());
                events.Add(key, EventDetails.Initialize(objRef.Name, occurrence));
            }

            EventDetails details = events[key];
            details.AddOccurrence(occurrence);

            Logger.LogDebug("Writing event to Kubernetes ({key}, Count: {count}", key.ToString(), details.Count);
            V1Event ev = new V1Event(objRef,
                new V1ObjectMeta() { Name = details.Name },
                action: action,
                message: message,
                reason: reason,
                firstTimestamp: details.FirstSeen,
                lastTimestamp: details.LastSeen,
                count: details.Count);

            Kubernetes.ReplaceNamespacedEventAsync(ev, details.Name, objRef.NamespaceProperty);
        }

        private class EventKey : IEquatable<EventKey>
        {
            public EventKey(string type, string action, string reason, string message, string involvedObject)
            {
                Type = type;
                Action = action;
                Reason = reason;
                Message = message;
                InvolvedObject = involvedObject;
            }

            public string Type { get; }
            public string Action { get; }
            public string Reason { get; }
            public string Message { get; }
            public string InvolvedObject { get; }

            public bool Equals(EventKey other)
            {
                return Type == other.Type && Action == other.Action && Reason == other.Reason && Message == other.Message && InvolvedObject == other.InvolvedObject;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + Type.GetHashCode();
                    hash = hash * 23 + Action.GetHashCode();
                    hash = hash * 23 + Reason.GetHashCode();
                    hash = hash * 23 + Message.GetHashCode();
                    hash = hash * 23 + InvolvedObject.GetHashCode();

                    return hash;
                }
            }

            public override string ToString()
            {
                return $"[{Type}]{InvolvedObject}-{Action}({Reason})";
            }
        }

        private class EventDetails
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public DateTime FirstSeen { get; set; }
            public DateTime LastSeen { get; set; }

            public static EventDetails Initialize(string name, DateTime firstSeen)
            {
                name = $"{name}-{Guid.NewGuid()}";

                return new EventDetails()
                {
                    Name = name,
                    FirstSeen = firstSeen
                };
            }

            public void AddOccurrence(DateTime time)
            {
                Count++;
                LastSeen = time;
            }
        }
    }


}
