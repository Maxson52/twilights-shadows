using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class FlashlightController : NetworkBehaviour
{
    private GameObject playerCamera;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            playerCamera = Camera.main.gameObject;

            // Set the flashlight to be a child of the camera
            transform.SetParent(playerCamera.transform);
<<<<<<< HEAD
            transform.localPosition = new Vector3(0, -0.1f, 0.35f);
=======
            transform.localPosition = new Vector3(0, -0.2f, 0.5f);
>>>>>>> f193adcae7e4d049c50bd895defe4986ea6bb97d
        }
    }

    void Update()
    {
<<<<<<< HEAD
        if (playerCamera == null)
            return;

        // Rotate the flashlight to match the camera's rotation
        transform.rotation = playerCamera.transform.rotation;

        // Randomly flicker the light
        if (Random.Range(0, 100) < 5) {
            GetComponent<Light>().intensity = Random.Range(0.5f, 6f);
        }
    }
}
=======
        // Rotate the flashlight to match the camera's rotation
        transform.rotation = playerCamera.transform.rotation;
    }
}
>>>>>>> f193adcae7e4d049c50bd895defe4986ea6bb97d
