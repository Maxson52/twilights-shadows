using UnityEngine;
using FishNet.Object;
 
public class NightVisionController : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            // Disable environment fog for this player
            RenderSettings.fog = false;
        } else {
            gameObject.SetActive(false);
        }
    }
}