using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Flashlight : MonoBehaviour
{
    private Light2D lightSource;
    public static Flashlight Build(GameObject parent)
    {
        return Utility.AttachChildObject(parent, "Flashlight").AddComponent<Flashlight>();
    }
    private void Start()
    {
        lightSource = gameObject.AddComponent<Light2D>();
        lightSource.lightType = Light2D.LightType.Freeform;
        Vector3[] shapePath = new Vector3[]{
            new Vector3(0,  0, 0) * 5,
            new Vector3(2, -1, 0) * 3,
            new Vector3(2,  1, 0) * 3,
        };
        lightSource.SetShapePath(shapePath);
        lightSource.intensity = 0.7f;
        lightSource.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) {
            lightSource.enabled = !lightSource.enabled;
        }
        Vector3 dir = Input.mousePosition - FindObjectOfType<Camera>().WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (FindObjectOfType<Pigbro>().transform.localScale.x >= 0)
        {
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            transform.rotation = Quaternion.AngleAxis(angle + 180, Vector3.forward);
        }
    }
}