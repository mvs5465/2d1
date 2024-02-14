using UnityEngine;

public class HomingDetector : MonoBehaviour
{
    private Rigidbody2D spellRigidbody;
    private float detectionRadius;
    private GameObject target;

    public static HomingDetector Build(GameObject parent, float detectionRadius)
    {
        GameObject hdgo = Utility.AttachChildObject(parent, "HomingDetector");
        HomingDetector homingDetector = hdgo.AddComponent<HomingDetector>();
        homingDetector.detectionRadius = detectionRadius;
        return homingDetector;
    }

    private void Start()
    {
        spellRigidbody = transform.parent.GetComponent<Rigidbody2D>();
        CircleCollider2D circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
        circleCollider2D.radius = detectionRadius;
        circleCollider2D.isTrigger = true;
    }

    private void SeekTarget()
    {
        if (target)
        {
            spellRigidbody.velocity = spellRigidbody.velocity.magnitude * (target.transform.position - transform.position).normalized;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity)
        {
            if (target)
            {
                if ((other.transform.position - transform.position).magnitude < (target.transform.position - transform.position).magnitude)
                {
                    target = other.gameObject;
                }
            }
            else
            {
                target = other.gameObject;
            }
            CancelInvoke(nameof(SeekTarget));
            InvokeRepeating(nameof(SeekTarget), 0.5f, 0.5f);
        }
    }
}