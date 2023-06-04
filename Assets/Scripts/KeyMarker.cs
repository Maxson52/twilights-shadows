using UnityEngine;
using UnityEngine.UI;

public class KeyMarker : MonoBehaviour
{
    public Sprite icon;
    public Image image;

    public Vector2 keyPosition {
        get { return new Vector2(transform.position.x, transform.position.z); }
    }
}
