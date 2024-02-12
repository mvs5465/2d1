using UnityEngine;

public static class HealthbarBuilder
{

    public class Healthbar : MonoBehaviour
    {
        private SpriteRenderer healthbarSpriteRenderer;
        private Vector3 originalSize;
        private Vector2 originalPosition;
        private void Start()
        {
            healthbarSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            originalSize = new Vector2(healthbarSpriteRenderer.size.x, healthbarSpriteRenderer.size.y);
            originalPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        }
        public void SetRatio(float ratio)
        {
            healthbarSpriteRenderer.drawMode = SpriteDrawMode.Tiled;
            healthbarSpriteRenderer.size = originalSize * new Vector2(ratio, 1);
            transform.localPosition = originalPosition * new Vector2(ratio, 1);
        }
    }
    public static Healthbar AttachHealthbar(GameConfig gameConfig, GameObject parent)
    {
        GameObject gameObject = Utility.AttachChildObject(parent, "Healthbar");

        // GameObject healthbarBackObj = Utility.AttachChildObject(gameObject, "HealthbarBack");
        // SpriteRenderer healthbarBackSr = healthbarBackObj.AddComponent<SpriteRenderer>();
        // healthbarBackSr.drawMode = SpriteDrawMode.Tiled;
        // healthbarBackSr.sprite = gameConfig.healthbarBackgroundSprite;
        // healthbarBackSr.sortingOrder = 1;
        // healthbarBackSr.color = new Color(1,1,1,0.8f);
        // healthbarBackObj.transform.localScale = new Vector2(2, 1f);
        // healthbarBackObj.transform.localPosition = new Vector2(0, 0.75f);

        GameObject healthbarObj = Utility.AttachChildObject(gameObject, "HealthbarFront");
        SpriteRenderer healthbarSpriteRenderer = healthbarObj.AddComponent<SpriteRenderer>();
        healthbarSpriteRenderer.drawMode = SpriteDrawMode.Tiled;
        healthbarSpriteRenderer.sprite = gameConfig.healthbarSprite;
        healthbarSpriteRenderer.sortingOrder = 2;
        healthbarSpriteRenderer.color = new Color(1,1,1,0.8f);
        Healthbar healthbar = healthbarObj.AddComponent<Healthbar>();
        healthbarObj.transform.localScale = new Vector2(2, 1f);
        healthbarObj.transform.localPosition = new Vector2(0, 0.75f);

        return healthbar;
    }
}