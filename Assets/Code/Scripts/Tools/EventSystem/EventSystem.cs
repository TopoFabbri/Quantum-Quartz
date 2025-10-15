using System;
using System.Collections.Generic;
using Code.Scripts.Tools.Pool;

namespace Code.Scripts.Tools.EventSystem
{
    public static class EventSystem
    {
        private static readonly Dictionary<Type, List<Delegate>> EventListeners = new();
        private static ConcurrentPool Pool { get; } = new();
        
        public static void Raise<TEvent>(params object[] parameters) where TEvent : IEvent, new()
        {
            TEvent eventInstance = Pool.Get<TEvent>(parameters);
            eventInstance.Assign(parameters);
            
            if (EventListeners.TryGetValue(eventInstance.GetType(), out List<Delegate> delegates))
            {
                foreach (Delegate del in delegates)
                    ((Action<TEvent>)del)?.Invoke(eventInstance);
            }
                
            Pool.Release(eventInstance);
        }

        public static void Subscribe<TEvent>(Action<TEvent> action) where TEvent : IEvent
        {
            if (!EventListeners.ContainsKey(typeof(TEvent)))
                EventListeners.Add(typeof(TEvent), new List<Delegate>());
            
            EventListeners[typeof(TEvent)].Add(action);
        }
        
        public static void Unsubscribe<TEvent>(Action<TEvent> action) where TEvent : IEvent
        {
            if (EventListeners.TryGetValue(typeof(TEvent), out List<Delegate> delegates))
                delegates.Remove(action);
        }
    }
}