using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class FlashlightColliderController : NetworkBehaviour
{
    public AudioClip flashlightSfx;

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
            Gotcha(other.gameObject);

            // Play sound
            AudioSource.PlayClipAtPoint(flashlightSfx, transform.position);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void Gotcha(GameObject seeker)
    {
        NetworkConnection conn = seeker.GetComponent<NetworkObject>().Owner;
        Vector3 loc = new Vector3(Random.Range(-270, 490), 16, Random.Range(30, 120));
        Quaternion rot = Quaternion.Euler(0, Random.Range(-90, 90), 0);

        seeker.GetComponent<FirstPersonController>().TeleportPlayer(conn, loc, rot);
    }
}