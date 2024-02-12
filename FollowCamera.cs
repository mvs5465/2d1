using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    private void Start()
    {
        transform.position = target.position + new Vector3(0, 0, -5);
        gameObject.GetComponent<Camera>().orthographicSize = 5;
    }

    private void FixedUpdate()
    {
        transform.position = target.position + new Vector3(0, 0, -5);
    }
}