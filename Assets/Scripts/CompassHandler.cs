using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class CompassHandler : MonoBehaviour 
{    
    public GameObject player;

    public GameObject keyMarkerPrefab;
    List<KeyMarker> keyMarkers = new List<KeyMarker>();
    
    float compassUnit;

    void Start() {
        compassUnit = gameObject.GetComponent<RawImage>().rectTransform.rect.width / 360f;
    }

    void Update() {
        foreach (KeyMarker keyMarker in keyMarkers) {
            if (keyMarker == null || keyMarker.image == null || keyMarker.image.rectTransform == null || player == null) {
                continue;
            }
            keyMarker.image.rectTransform.anchoredPosition = GetPositionOnCompass(keyMarker);
        }
    }

    public void AddKeyMarker(KeyMarker keyMarker) {
        GameObject newKeyMarker = Instantiate(keyMarkerPrefab, gameObject.GetComponent<RawImage>().transform);
        keyMarker.image = newKeyMarker.GetComponent<Image>();
        keyMarker.image.sprite = keyMarker.icon;

        keyMarkers.Add(keyMarker);
    }

    public void RemoveKeyMarker(GameObject key) {
        foreach (KeyMarker keyMarker in keyMarkers) {
            if (keyMarker == key.GetComponent<KeyMarker>()) {
                keyMarkers.Remove(keyMarker);
                Destroy(keyMarker.image.gameObject);
                break;
            }
        }
    }

    Vector2 GetPositionOnCompass(KeyMarker keyMarker) {
        Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerForward = new Vector2(player.transform.forward.x, player.transform.forward.z);

        float angle = Vector2.SignedAngle(keyMarker.keyPosition - playerPosition, playerForward);

        return new Vector2(angle * compassUnit, 0f);
    }
}