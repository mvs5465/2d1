using UnityEngine;
using UnityEngine.Tilemaps;

public class SlopeDetector : MonoBehaviour
{
    private float detectorSize;
    private bool detectedSlope = false;

    public static SlopeDetector Build(GameObject parent, Vector2 localPosition, float size)
    {
        GameObject sdgo = Utility.AttachChildObject(parent, "SlopeDetector");
        sdgo.transform.localPosition = localPosition;
        SlopeDetector slopeDetector = sdgo.AddComponent<SlopeDetector>();
        slopeDetector.detectorSize = size;
        return slopeDetector;
    }

    private void Start()
    {
        BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = true;
        boxCollider2D.size = Vector2.one * detectorSize;
    }

    public bool Detect()
    {
        return detectedSlope;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Tilemap>())
        {
            Debug.Log(gameObject.name + " detected slope!");
            detectedSlope = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Tilemap>())
        {
            detectedSlope = false;
        }
    }
}