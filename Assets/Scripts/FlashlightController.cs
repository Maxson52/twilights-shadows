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
            transform.localPosition = new Vector3(0, -0.2f, 0.5f);
        }
    }

    void Update()
    {
        // Rotate the flashlight to match the camera's rotation
        transform.rotation = playerCamera.transform.rotation;

        // Randomly flicker intensity of he flashlight
        if (Random.Range(0, 25) == 1) {
            GetComponent<Light>().intensity = Random.Range(1.2f, 1.6f);
        }
    }
}
