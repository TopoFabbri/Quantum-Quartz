using System;
using System.Collections.Concurrent;

namespace Code.Scripts.Tools.Pool
{
    public class ConcurrentPool
    {
        private readonly ConcurrentDictionary<Type, ConcurrentStack<IResetable>> Pool = new();

        public TResetable Get<TResetable>(params object[] parameters) where TResetable : IResetable, new()
        {
            if (!Pool.ContainsKey(typeof(TResetable)))
                Pool.TryAdd(typeof(TResetable), new ConcurrentStack<IResetable>());

            TResetable value;

            if (Pool[typeof(TResetable)].Count > 0)
            {
                Pool[typeof(TResetable)].TryPop(out IResetable resetable);
                value = (TResetable)resetable;
            }
            else
            {
                value = new TResetable();
            }

            value.Assign(parameters);
            return value;
        }

        public void Release<TResetable>(TResetable obj) where TResetable : IResetable, new()
        {
            obj.Reset();
            Pool[typeof(TResetable)].Push(obj);
        }
    }
}