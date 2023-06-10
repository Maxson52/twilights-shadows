using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    public AudioClip KeyCollectAudioClip;

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

    void Update()
    {
        transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Add key function
            GameObject.Find("GameStateManager").GetComponent<GameStateManager>().KeyCollected(gameObject);
            AudioSource.PlayClipAtPoint(KeyCollectAudioClip, transform.position);
        }
    }
}
