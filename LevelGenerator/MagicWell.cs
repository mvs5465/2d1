using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MagicWell : MonoBehaviour
{
    public MagicWellData wellData;
    private Light2D wellLight;
    private bool inRange;

    private void Start()
    {
        GameObject spellDisplayContainer = Utility.AttachChildObject(gameObject, "SpellAnimator");
        SpriteAnimator.Build(spellDisplayContainer, wellData.magicAnimation);
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = 1.2f;
        collider.isTrigger = true;

        wellLight = spellDisplayContainer.AddComponent<Light2D>();
        wellLight.lightType = Light2D.LightType.Point;
        wellLight.intensity = 0.75f;
        wellLight.pointLightInnerRadius = 1;
        wellLight.pointLightOuterRadius = 3;
        wellLight.color = Color.white + Color.green;
    }

    private void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            FindObjectOfType<LevelGenerator>().GenerateCave();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Wizard>())
        {
            wellLight.intensity = 1;
            wellLight.pointLightInnerRadius = 1;
            wellLight.pointLightOuterRadius = 4;
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Wizard>())
        {
            wellLight.intensity = 0.5f;
            wellLight.pointLightInnerRadius = 1;
            wellLight.pointLightOuterRadius = 3;
            inRange = false;
        }
    }
}