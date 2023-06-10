using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class TurnOffFlashlight : NetworkBehaviour
{
    public AudioClip booSfx;
    public AudioClip shhSfx;

    private float timeToTurnOff = 2f;

    private bool haveShhed = false;

    void Update()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            if (Vector3.Distance(transform.position, player.transform.position) < 2.5f && GameObject.Find("GameStateManager").GetComponent<GameStateManager>().gameOn) {
                // play shhh audio
                if (!haveShhed) {
                    PlayShh(GetComponent<NetworkObject>().Owner);
                    haveShhed = true;
                }

                timeToTurnOff -= Time.deltaTime;

                if (timeToTurnOff < 0) {
                    ServerTurnOffFlashlight();
                    timeToTurnOff = 60f;
                    // wait 0.5 seconds before changing the winner text
                    Invoke("ChangeWinnerText", 1.5f);
                }
            } else {
                timeToTurnOff = 2f;
                haveShhed = false;
            }
        }
    }

    void ChangeWinnerText() {
        GameObject.Find("GameStateManager").GetComponent<GameStateManager>().Winner("Seekers win!");
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerTurnOffFlashlight() {
        RpcTurnOffFlashlight();
    }

    [ObserversRpc]
    public void RpcTurnOffFlashlight() {
        // play boo audio
        GameObject.Find("BG Music").GetComponent<AudioSource>().clip = booSfx;
        GameObject.Find("BG Music").GetComponent<AudioSource>().loop = false;
        GameObject.Find("BG Music").GetComponent<AudioSource>().Play();

        // turn off flashlight
        foreach (GameObject flashlight in GameObject.FindGameObjectsWithTag("Flashlight")) {
            flashlight.SetActive(false);
        }
    }

    [TargetRpc]
    void PlayShh(NetworkConnection conn) {
        AudioSource.PlayClipAtPoint(shhSfx, transform.position);
    }
}
