using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class TurnOffFlashlight : NetworkBehaviour
{
    public AudioClip booSfx;

    private float timeToTurnOff = 2f;

    void Update()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            if (Vector3.Distance(transform.position, player.transform.position) < 2f) {
                timeToTurnOff -= Time.deltaTime;
                if (timeToTurnOff < 0 && GameObject.Find("GameStateManager").GetComponent<GameStateManager>().gameOn) {
                    ServerTurnOffFlashlight();
                    GameObject.Find("GameStateManager").GetComponent<GameStateManager>().ServerTurnOffFlashlight();
                    timeToTurnOff = 60f;
                }
            } else {
                timeToTurnOff = 2f;
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
