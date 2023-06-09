using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Seeker")
        {
            // Pick a random powerup
            int powerUp = Random.Range(1, 4);

            if (powerUp == 1)
            {
                ChangeSpeed(other.gameObject, 5f);
            }
            else if (powerUp == 2)
            {
                ChangeTime(10f);
            }
            else if (powerUp == 3)
            {
                ChangeFog(other.gameObject, 0.08f);
            }

            Destroy(gameObject);
        } else if (other.gameObject.CompareTag("Terrain"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    void ChangeSpeed(GameObject player, float amount)
    {
        if (player.GetComponent<FirstPersonController>().runningSpeed >= 19) return;

        Debug.Log("Changing speed");
        player.GetComponent<FirstPersonController>().walkingSpeed += amount;
        player.GetComponent<FirstPersonController>().runningSpeed += amount;
        StartCoroutine(ResetSpeed(player, amount));
    }
    IEnumerator ResetSpeed(GameObject player, float amount)
    {
        // Reset speed after 5 seconds
        Debug.Log("Waiting to reset speed");
        yield return new WaitForSeconds(5f);
        Debug.Log("Resetting speed");
        player.GetComponent<FirstPersonController>().walkingSpeed -= amount;
        player.GetComponent<FirstPersonController>().runningSpeed -= amount;
    }

    void ChangeTime(float amount)
    {
        Debug.Log("Changing time");
        GameObject.Find("GameStateManager").GetComponent<GameStateManager>().ChangeTime(amount);
    }

    void ChangeFog(GameObject player, float amount)
    {
        Debug.Log("Changing fog");
        RenderSettings.fogDensity += amount;
        StartCoroutine(ResetFog(player, amount));
    }
    IEnumerator ResetFog(GameObject player, float amount)
    {
        Debug.Log("Waiting to reset fog");
        yield return new WaitForSeconds(5f);
        Debug.Log("Resetting fog");
        RenderSettings.fogDensity -= amount;
    }
}
