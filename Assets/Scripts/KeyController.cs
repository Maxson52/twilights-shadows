using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Key collected!");

            GameObject.Find("GameStateManager").GetComponent<GameStateManager>().keysCollected++;

            Destroy(gameObject);
        } else if (other.gameObject.CompareTag("Terrain")) 
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
