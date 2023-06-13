using System;
using System.Collections.Generic;

namespace beepe0.UNetwork
{
    public static class UNetworkUpdate
    {
        private static readonly List<Action> Execute = new List<Action>();
        private static readonly List<Action> ExecuteCopied = new List<Action>();
        private static bool _executeOnMainThread;
    
        public static void AddToQueue(Action action)
        {
            if (action == null)
            {
                UNetworkLogs.ErrorNullFunc();
                return;
            }

            lock (Execute)
            {
                Execute.Add(action);
                _executeOnMainThread = true;
            }
        }
        public static void Update()
        {
            if (_executeOnMainThread)
            {
                ExecuteCopied.Clear();
                lock (Execute)
                {
                    ExecuteCopied.AddRange(Execute);
                    Execute.Clear();

                    _executeOnMainThread = false;
                }

                ExecuteCopied.ForEach((action) => { action(); });
            }
        }
    }
}