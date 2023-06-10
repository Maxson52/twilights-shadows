using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PowerUpController : NetworkBehaviour
{
    public AudioClip powerUpSfx;

    void Start()
    {
        StartCoroutine(EnablePowerUp());
    }

    IEnumerator EnablePowerUp()
    {
        yield return new WaitForSeconds(3f);
        GetComponent<BoxCollider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Seeker")
        {
            // Pick a random powerup
            int powerUp = Random.Range(1, 4);
            int buffOrDebuff = Random.Range(0, 1);

            if (powerUp == 1)
            {
                float amount = buffOrDebuff == 0 ? 5f : -5f;

                ChangeSpeed(other.gameObject, amount);
            }
            else if (powerUp == 2)
            {
                float amount = buffOrDebuff == 0 ? 10f : -10f;

                ChangeTime(amount);
            }
            else if (powerUp == 3)
            {
                float amount = buffOrDebuff == 0 ? 0.08f : -0.08f;

                ChangeFog(other.gameObject, amount);
            }

            // Play sound
            AudioSource.PlayClipAtPoint(powerUpSfx, transform.position);

            // Hide powerup
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void ChangeSpeed(GameObject player, float amount)
    {
        if (player.GetComponent<FirstPersonController>().runningSpeed >= 19) return;

        player.GetComponent<FirstPersonController>().walkingSpeed += amount;
        player.GetComponent<FirstPersonController>().runningSpeed += amount;
        StartCoroutine(ResetSpeed(player, amount));
    }
    IEnumerator ResetSpeed(GameObject player, float amount)
    {
        // Reset speed after 5 seconds
        yield return new WaitForSeconds(5f);
        Debug.Log("Resetting speed " + amount);
        player.GetComponent<FirstPersonController>().walkingSpeed -= amount;
        player.GetComponent<FirstPersonController>().runningSpeed -= amount;

        DestroyPowerUpServerRpc();
    }

    void ChangeTime(float amount)
    {
        GameObject.Find("GameStateManager").GetComponent<GameStateManager>().ChangeTime(amount);
        DestroyPowerUpServerRpc();
    }

    void ChangeFog(GameObject player, float amount)
    {
        RenderSettings.fogDensity += amount;
        StartCoroutine(ResetFog(player, amount));
    }
    IEnumerator ResetFog(GameObject player, float amount)
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("Resetting fog " + amount);
        RenderSettings.fogDensity -= amount;
        
        DestroyPowerUpServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyPowerUpServerRpc()
    {
        DestroyPowerUpObserversRpc();
    }
    [ObserversRpc]
    public void DestroyPowerUpObserversRpc()
    {
        Destroy(gameObject);
    }
}
