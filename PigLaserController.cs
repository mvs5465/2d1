using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class PigLaserController : MonoBehaviour
{
    private List<Entity> targets;
    private Entity currentTarget;
    private LineRenderer lr;
    private bool firing = false;

    public static void BuildAndAttach(GameObject parent, GameConfig gameConfig)
    {
        GameObject pigLaserContainer = Utility.AttachChildObject(parent, "PigLaserContainer");
        pigLaserContainer.AddComponent<PigLaserController>();
        SpriteRenderer sr = pigLaserContainer.AddComponent<SpriteRenderer>();
        sr.sprite = gameConfig.pigLaserSprite;
        pigLaserContainer.transform.localScale = Vector2.one / 1.5f;
        pigLaserContainer.transform.localPosition = Vector2.up*0.9f;
    }
    private void Start()
    {
        targets = new List<Entity>() { };
        lr = gameObject.AddComponent<LineRenderer>();
        CircleCollider2D detectorCollider = gameObject.AddComponent<CircleCollider2D>();
        detectorCollider.isTrigger = true;
        detectorCollider.radius = 5;

        Debug.Log("PigLaserController Ready");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Browner>())
        {
            targets.Add(other.GetComponent<Browner>());
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Browner>())
        {
            targets.Remove(other.GetComponent<Browner>());
        }
    }

    private void FixedUpdate()
    {
        // Shortcut to keep firing at target
        if (firing && targets.Count() > 0 && targets.First() == currentTarget)
        {
            return;
        }
        // Check for new targets
        else if (
            (!firing && targets.Count() > 0) ||
            (firing && targets.Count() > 0 && targets.First() != currentTarget))
        {
            currentTarget = targets.First();
            Vector3[] positions = { transform.position + Vector3.up * 0.75f, currentTarget.transform.position };
            lr.enabled = true;
            lr.SetPositions(positions);
            lr.startWidth = 0.05f;
            lr.endWidth = 0.2f;
            Material whiteDiffuseMat = new(Shader.Find("Unlit/Texture"))
            {
                color = Color.red
            };
            lr.material = whiteDiffuseMat;
            if (!firing)
            {
                firing = true;
                InvokeRepeating(nameof(FireLaser), 0, 0.1f);
            }
        }
        // Stop firing if no targets
        else if (firing && targets.Count() == 0)
        {
            lr.enabled = false;
            firing = false;
            CancelInvoke(nameof(FireLaser));
        }
    }
    private void FireLaser()
    {
        if (currentTarget && firing)
        {
            Vector3[] positions = { transform.position + Vector3.up * 0.75f, currentTarget.transform.position };
            lr.SetPositions(positions);
            currentTarget.Damage(1);
        }
    }
}