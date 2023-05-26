using UnityEngine;
using FishNet.Object;
 
public class NightVisionController : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            RenderSettings.fogDensity = 0.025f;
        } else {
            gameObject.SetActive(false);
        }
    }
}