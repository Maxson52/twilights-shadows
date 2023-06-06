using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class FlashlightColliderController : NetworkBehaviour
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
            transform.localPosition = new Vector3(0, 0, 5.5f);
        }
    }

    // on trigger enter with seeker, place seeker randomly on the map
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Seeker") && GameObject.Find("GameStateManager").GetComponent<GameStateManager>().gameOn)
        {   
            Debug.Log("Seeker entered flashlight collider");
            GameObject.Find("GameStateManager").GetComponent<GameStateManager>().PlaceSeeker(other.gameObject);
        }
    }
}