using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class GameStateManager : NetworkBehaviour
{
    [SerializeField]
    private float timeToStart = 30f;
    [SerializeField]
    private float timeToPlay = 300f;

    [SyncVar]
    private float timeRemaining = 0;
    [SerializeField] private TextMeshProUGUI timerText;
    bool gameOn = false;

    void Start()
    {
        timeRemaining = timeToStart;
    }

    void Update()
    {       
        if (gameOn) {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timerText.text = "Time remaining: " + Mathf.Round(timeRemaining);
            }
            else
            {
                timerText.text = "Game over!";
            }
        }
        else {
            // Only start if there is at least one object with tag Player and one Seeker
            if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    timerText.text = "Starting in: " + Mathf.Round(timeRemaining);
                }
                else
                {
                    timerText.text = "Starting now...";
                    StartGame();
                    gameOn = true;
                }
            }
        }
    }

    void StartGame() 
    {
        timeRemaining = timeToPlay;

        // Place players randomly between z = 30 and z = 120 (x is -290 and y = 12)
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = new Vector3(-290, 12, Random.Range(30, 120));
            player.GetComponent<CharacterController>().enabled = true;
            // facing the +x direction
            player.transform.rotation = Quaternion.Euler(0, 90, 0);
       }

        // Place seekers randomly between z = 30 and z = 120 (x is 470 and y = 12)
        foreach (GameObject seeker in GameObject.FindGameObjectsWithTag("Seeker"))
        {
            seeker.GetComponent<CharacterController>().enabled = false;
            seeker.transform.position = new Vector3(470, 12, Random.Range(30, 120));
            seeker.GetComponent<CharacterController>().enabled = true;
            // facing the -x direction
            seeker.transform.rotation = Quaternion.Euler(0, -90, 0);
        }
    }
}
