using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class GameStateManager : NetworkBehaviour
{
    [SyncVar]
    public float timeRemaining = 30f;
    [SerializeField] private TextMeshProUGUI timerText;

    void Update()
    {
        // if (!base.IsServer) return;

        // Only start if there is at least one object with tag Player and one Seeker
        if (GameObject.FindGameObjectsWithTag("Player").Length > 0 && GameObject.FindGameObjectsWithTag("Seeker").Length > 0)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timerText.text = "Starting in: " + Mathf.Round(timeRemaining);
            }
            else
            {
                timerText.text = "Starting now...";
            }
        }
    }
}
