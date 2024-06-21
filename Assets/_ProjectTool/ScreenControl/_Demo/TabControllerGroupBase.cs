using _ProjectTool;
using UnityEngine;

namespace _ProjectTool
{
  public abstract class TabControllerGroupBase<TActor> : MonoBehaviour where TActor : MonoBehaviour,IActor
{
  public void Resister(TabActorController<TActor> controller)
  {
    controller.OnOpen += OnOpenHandle;
    controller.OnClose += OnCloseHandle;
    controller.OnCloseImmediate += OnCloseImmediateHandle;
  }
  public void UnResister(TabActorController<TActor> controller)
  {
    controller.OnOpen -= OnOpenHandle;
    controller.OnClose -= OnCloseHandle;
    controller.OnCloseImmediate -= OnCloseImmediateHandle;
  }

  protected virtual void OnOpenHandle(TabActorEventNotifier<TActor>.TabActorEventInfo info)
  {
    
  }
  protected virtual void OnCloseHandle(TabActorEventNotifier<TActor>.TabActorEventInfo info)
  {
    
  }
  protected virtual void OnCloseImmediateHandle(TabActorEventNotifier<TActor>.TabActorEventInfo info)
  {
    
  }
}
    
}
