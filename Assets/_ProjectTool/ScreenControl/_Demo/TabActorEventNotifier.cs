using System;
using UnityEngine;

namespace _ProjectTool
{
    public class TabActorEventNotifier<TActor> where TActor : MonoBehaviour, IActor
    {
        public event Action<TabActorEventInfo> OnOpen;
        public event Action<TabActorEventInfo> OnClose;
        public event Action<TabActorEventInfo> OnCloseImmediate;

        public void NotifyOpen(TabActorController<TActor> tabActorController, string identifier)
        {
            OnOpen?.Invoke(new TabActorEventInfo(tabActorController, identifier));
        }

        public void NotifyClose(TabActorController<TActor> tabActorController, string identifier)
        {
            OnClose?.Invoke(new TabActorEventInfo(tabActorController, identifier));
        }

        public void NotifyCloseImmediate(TabActorController<TActor> tabActorController, string identifier)
        {
            OnCloseImmediate?.Invoke(new TabActorEventInfo(tabActorController, identifier));
        }

        public struct TabActorEventInfo
        {
            public TabActorController<TActor> TabActorController { get; }
            public string Identifier { get; }

            public TabActorEventInfo(TabActorController<TActor> tabActorController, string identifier)
            {
                this.Identifier = identifier;
                TabActorController = tabActorController;
            }
        }
    }
}