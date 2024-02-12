using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Spellbook : MonoBehaviour
{
    public SpellData spellData;
    private Rigidbody2D rb;
    private bool inRange;
    private Light2D bookLight;

    public static Spellbook Build(SpellData spellData, Vector2 location)
    {
        GameObject spellbookContainer = new("Spellbook");
        spellbookContainer.transform.position = location;
        Spellbook spellbook = spellbookContainer.AddComponent<Spellbook>();
        spellbook.spellData = spellData;
        return spellbook;
    }

    private void Start()
    {
        GameObject spellDisplayContainer = Utility.AttachChildObject(gameObject, "SpellAnimator");
        SpriteAnimator.Build(spellDisplayContainer, spellData.GetSpellbookAnimation(), "Effect");
        spellDisplayContainer.transform.localScale *= spellData.spellDisplaySize;
        spellDisplayContainer.transform.localPosition += Vector3.up * 0.75f;
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = 1.2f;
        collider.isTrigger = true;
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.velocity = Vector2.up * 0.1f;

        bookLight = spellDisplayContainer.AddComponent<Light2D>();
        bookLight.lightType = Light2D.LightType.Point;
        bookLight.intensity = 0.75f;
        bookLight.pointLightInnerRadius = 1;
        bookLight.pointLightOuterRadius = 3;
        bookLight.color = Color.white + Color.yellow;

        InvokeRepeating(nameof(Hover), 0.5f, 1);
    }

    private void Hover()
    {
        rb.velocity *= -1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Wizard>())
        {
            bookLight.intensity = 1;
            bookLight.pointLightInnerRadius = 1;
            bookLight.pointLightOuterRadius = 4;
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Wizard>())
        {
            bookLight.intensity = 0.5f;
            bookLight.pointLightInnerRadius = 1;
            bookLight.pointLightOuterRadius = 3;
            inRange = false;
        }
    }

    private void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            spellData.Pickup();
            Destroy(gameObject);
        }
    }
}