using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Candle : MonoBehaviour
{
    public CandleData candleData;

    private SpriteAnimator spriteAnimator;
    private Light2D candleLight;
    private GameObject target;
    public static Candle Build(GameObject parent, CandleData candleData)
    {
        GameObject candleContainer = new("Candle");
        Candle candle = candleContainer.AddComponent<Candle>();
        candle.transform.localPosition += Vector3.up;
        candle.candleData = candleData;
        return candle;
    }
    private void Start()
    {
        target = FindObjectOfType<Wizard>().gameObject;
        spriteAnimator = SpriteAnimator.Build(gameObject, candleData.candleOff);
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Effect";
        gameObject.transform.localScale = Vector3.one * 0.6f;

        gameObject.layer = candleData.layer;
        candleLight = gameObject.AddComponent<Light2D>();
        candleLight.lightType = Light2D.LightType.Point;
        candleLight.intensity = 0.8f;
        candleLight.pointLightInnerRadius = 5f;
        candleLight.pointLightOuterRadius = 12;
        candleLight.color = Color.white;
        candleLight.falloffIntensity = 0.7f;
        candleLight.enabled = false;

        gameObject.AddComponent<CircleCollider2D>();
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0.01f;
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        Vector3 targetLead = target.GetComponent<Rigidbody2D>().velocity*Vector2.right;
        Vector2 targetPosition = target.transform.position - transform.position + Vector3.up * 1.5f + targetLead;
        float targetDistance = targetPosition.magnitude;
        if (targetDistance > 0.1f)
        {
            gameObject.GetComponent<CircleCollider2D>().enabled = targetDistance < 5;
            gameObject.GetComponent<Rigidbody2D>().velocity = targetPosition + target.GetComponent<Rigidbody2D>().velocity*0.7f;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!candleLight.enabled)
            {
                candleLight.enabled = true;
                spriteAnimator.SetAnimation(candleData.candleStart);
                Invoke(nameof(CandleLit), spriteAnimator.GetDuration());
            }
            else
            {
                candleLight.enabled = false;
                spriteAnimator.SetAnimation(candleData.candleOff);
            }

        }
    }

    private void CandleLit()
    {
        if (candleLight.enabled && spriteAnimator.GetAnimation() != candleData.candleLit)
        {
            spriteAnimator.SetAnimation(candleData.candleLit);
        }
    }

    private void Light()
    {
        spriteAnimator.SetAnimation(candleData.candleStart);
    }
}