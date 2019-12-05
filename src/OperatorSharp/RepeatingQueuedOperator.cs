using k8s;
using Microsoft.Extensions.Logging;
using OperatorSharp.CustomResources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OperatorSharp
{
    public abstract class RepeatingQueuedOperator<TCustomResource> : Operator<TCustomResource>, IDisposable where TCustomResource : CustomResource
    {
        private ConcurrentQueue<CustomResourceExecutionContext<TCustomResource>> executionQueue { get; set; } = new ConcurrentQueue<CustomResourceExecutionContext<TCustomResource>>();
        private ConcurrentDictionary<CustomResourceExecutionContext<TCustomResource>, CustomResourceExecutionContext<TCustomResource>> retryItems { get; set; } 
            = new ConcurrentDictionary<CustomResourceExecutionContext<TCustomResource>, CustomResourceExecutionContext<TCustomResource>>();

        private Timer timer;
        private readonly int? executionLimit;

        public RepeatingQueuedOperator(IKubernetes client, ILogger<Operator<TCustomResource>> logger, int delayMilliseconds, int periodMilliseconds, int? executionLimit = null) : base(client, logger)
        {
            timer = new Timer(HandleTimer, null, delayMilliseconds, periodMilliseconds);
            this.executionLimit = executionLimit;
        }

        public override void HandleItem(WatchEventType eventType, TCustomResource item)
        {
            var executionContext = new CustomResourceExecutionContext<TCustomResource>() { Item = item, EventType = eventType, PreviousExecutionsCount = 0 };
            executionQueue.Enqueue(executionContext);
        }

        protected void HandleTimer(object state)
        {
            var now = DateTimeOffset.Now;
            var newEvents = retryItems.Where(k => k.Value.NextExecutionTime <= now).Select(k => k.Value).ToList();
            foreach (var ev in newEvents)
            {
                if (retryItems.TryRemove(ev, out _))
                {
                    Logger.LogDebug("Queueing {kind} {item} from retry list", ev.Item.Kind, ev.Item.Metadata.Name);
                    executionQueue.Enqueue(ev);
                }
            }

            if (executionQueue.TryDequeue(out var context))
            {
                try
                {
                    Logger.LogDebug("Dequeueing {kind} {name} for execution ({n}th execution since {enqueueDate})", context.Item.Kind, context.Item.Metadata.Name, context.PreviousExecutionsCount);
                    var result = HandleDequeuedItem(context.EventType, context.Item, context.PreviousExecutionsCount);
                    if (!result)
                    {
                        Logger.LogDebug("Execution of {kind} {name} failed", context.Item.Kind, context.Item.Metadata.Name);
                        RequeueContext(context);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("An error occurred while processing the message. The message will be requeued", ex);

                    RequeueContext(context);
                }
            }
        }

        private void RequeueContext(CustomResourceExecutionContext<TCustomResource> context)
        {
            if (executionLimit == null || context.PreviousExecutionsCount < executionLimit)
            {
                context.PreviousExecutionsCount++;
                var backoffFactor = Math.Min(context.PreviousExecutionsCount, 6);
                var backoffSeconds = Math.Pow(2, backoffFactor);
                context.NextExecutionTime = DateTimeOffset.Now.AddSeconds(backoffSeconds);

                Logger.LogDebug("Requeuing {kind} {name} to execute after {time} ({backoffSeconds})", context.Item.Kind, context.Item.Metadata.Name, context.NextExecutionTime, backoffSeconds);
                retryItems.TryAdd(context, context);
            }
        }

        public abstract bool HandleDequeuedItem(WatchEventType type, TCustomResource item, int executionCount);

        public void Dispose()
        {
            timer.Dispose();
        }
    }

    public class CustomResourceExecutionContext<TCustomResource> where TCustomResource : CustomResource
    {
        public TCustomResource Item { get; set; }
        public WatchEventType EventType { get; set; }
        public int PreviousExecutionsCount { get; set; }

        public DateTimeOffset EnqueuedDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? NextExecutionTime { get; set; }
    }
}
