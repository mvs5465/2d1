using UnityEngine;

public class AggroManager : MonoBehaviour
{
    public float aggroRadius;
    private GameObject target;
    private Rigidbody2D parentRigidBody;
    public static void BuildAndAttach(Browner parent, int aggroRadius)
    {
        GameObject enemyControllerContainer = Utility.AttachChildObject(parent.gameObject, "EnemyController");
        AggroManager enemyController = enemyControllerContainer.AddComponent<AggroManager>();
        enemyController.aggroRadius = aggroRadius;
    }
    private void Start()
    {
        CircleCollider2D circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
        circleCollider2D.isTrigger = true;
        circleCollider2D.radius = aggroRadius;

        parentRigidBody = gameObject.GetComponentInParent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Wizard>())
        {
            target = other.gameObject;
            InvokeRepeating(nameof(Pursue), 0.5f, 1f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Wizard>())
        {
            CancelInvoke(nameof(Pursue));
            target = null;
        }
    }

    private void Pursue()
    {
        Vector2 pursuitVector = target.transform.position - transform.position;
        pursuitVector /= pursuitVector.magnitude;
        parentRigidBody.AddForce(pursuitVector, ForceMode2D.Impulse);
    }
}