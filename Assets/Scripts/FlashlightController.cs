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
            transform.localPosition = new Vector3(0, 0, 0.4f);
        }
    }

    void Update()
    {
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