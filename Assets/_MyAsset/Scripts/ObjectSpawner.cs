using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSpawner<TActor> : MonoBehaviour where TActor : MonoBehaviour,IActor
{
    private Dictionary<string, TActor> _actorCash = new Dictionary<string, TActor>();
    
    public Task Open(string key)
    {
        return OpenAsync(key);
    }

    private async Task OpenAsync(string key)
    {
        var load = Resources.Load(key);
        var spawn=Instantiate(load);
        var actor=spawn.GetOrAddComponent<TActor>();
        await actor.Open();
        _actorCash.Add(key,actor);
    }
    public Task Close(string key)
    {
        return CloseAsync(key);
    }
    private async Task CloseAsync(string key)
    {
        if(_actorCash.ContainsKey(key))return;
        var close = _actorCash[key];
        await close.Close();
        Destroy(close);
        _actorCash.Remove(key);
        Resources.UnloadAsset(close);
    }
}
public interface IActor
{
   Task Open();
   Task Close();
}