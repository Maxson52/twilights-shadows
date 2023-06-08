using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class CompassHandler : MonoBehaviour 
{    
    public GameObject player;

    public GameObject iconPrefab;

    List<CompassMarker> markers = new List<CompassMarker>();
    
    float compassUnit;

    void Start() {
        compassUnit = gameObject.GetComponent<RawImage>().rectTransform.rect.width / 360f;
    }

    void Update() {
        foreach (CompassMarker marker in markers) {
            if (marker == null || marker.image == null || marker.image.rectTransform == null || player == null) {
                continue;
            }
            marker.image.rectTransform.anchoredPosition = GetPositionOnCompass(marker);
        }
    }

    public void AddMarker(CompassMarker marker) {
        GameObject newMarker = Instantiate(iconPrefab, gameObject.GetComponent<RawImage>().transform);
        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;

        markers.Add(marker);
    }

    public void ShowMarker(GameObject markerObject) {
        CompassMarker marker = markerObject.GetComponent<CompassMarker>();
        if (marker == null || marker.image == null) return;
        marker.image.enabled = true;
    }

    public void RemoveMarker(GameObject markerObject) {
        CompassMarker marker = markerObject.GetComponent<CompassMarker>();
        if (marker == null || marker.image == null) return;
        marker.image.enabled = false;
    }

    Vector2 GetPositionOnCompass(CompassMarker marker) {
        Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerForward = new Vector2(player.transform.forward.x, player.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - playerPosition, playerForward);

        return new Vector2(angle * compassUnit, 0f);
    }
}