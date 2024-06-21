using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSpawner<TActor> : MonoBehaviour where TActor : MonoBehaviour,IActor
{
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
    }
    public Task Close(string key)
    {
        return CloseAsync(key);
    }
    private async Task CloseAsync(string key)
    {
        
    }
}
public interface IActor
{
   Task Open();
   Task Close();
}