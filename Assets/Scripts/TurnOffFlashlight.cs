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
            if (Vector3.Distance(transform.position, player.transform.position) < 2f && GameObject.Find("GameStateManager").GetComponent<GameStateManager>().gameOn) {
                // play shhh audio
                if (!haveShhed) {
                    AudioSource.PlayClipAtPoint(shhSfx, transform.position);                            
                    haveShhed = true;
                }

                timeToTurnOff -= Time.deltaTime;

                if (timeToTurnOff < 0) {
                    ServerTurnOffFlashlight();
                    GameObject.Find("GameStateManager").GetComponent<GameStateManager>().Winner("Seekers win!");
                    timeToTurnOff = 60f;
                }
            } else {
                timeToTurnOff = 2f;
                haveShhed = false;
            }
        }
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
}
