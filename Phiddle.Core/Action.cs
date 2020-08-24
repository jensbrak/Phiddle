using System.Collections.Generic;
using System;

namespace Phiddle.Core
{

    public delegate void ActionDelegate();

    public class AppActions<T> 
    {
        private Dictionary<T, ActionDelegate> actions;

        public AppActions()
        {
            actions = new Dictionary<T, ActionDelegate>();
        }

        public AppActions(Dictionary<T, ActionDelegate> actions) : base()
        {
            this.actions = actions;
        }

        public void Add(T key, ActionDelegate action)
        {
            actions.Add(key, action);
        }

        public bool Has(T key)
        {
            return actions.ContainsKey(key);
        }

        public void Invoke(T actionId)
        {
            actions[actionId].Invoke();
        }
    }
}
