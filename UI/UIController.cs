using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public GameConfig gameConfig;
    private GameObject uiContainer;
    private Pigbro player;

    private void Start()
    {
        player = FindObjectOfType<Pigbro>();
        Invoke(nameof(Draw), 1f);
    }
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Tab))
    //     {
    //         if (!uiContainer)
    //         {
    //             Draw();
    //         }
    //         else
    //         {
    //             Erase();
    //         }
    //     }
    // }

    public void ReDraw()
    {
        if (uiContainer) Draw();
    }

    private void Draw()
    {
        if (uiContainer) Destroy(uiContainer);
        Camera camera = FindObjectOfType<Camera>();
        uiContainer = Utility.AttachChildObject(camera.gameObject, "UIContainer");

        GameObject restartButtonContainer = Utility.AttachChildObject(uiContainer, "UIHealthText");
        UIDocument uiDocument = restartButtonContainer.AddComponent<UIDocument>();
        uiDocument.panelSettings = gameConfig.panelSettings;
        uiDocument.visualTreeAsset = gameConfig.uiAsset;

        if (player.GetHealth() == 0)
        {
            VisualElement gameOverWidget = new();
            uiDocument.rootVisualElement.Add(gameOverWidget);
            gameOverWidget.style.position = Position.Absolute;
            gameOverWidget.style.top = 50;
            gameOverWidget.style.left = 0;
            gameOverWidget.style.right = 0;
            gameOverWidget.style.backgroundColor = Color.white;
            gameOverWidget.style.alignItems = Align.Center;

            Label gameOverLabel = new();
            gameOverLabel.text = "You died! Return to base?";
            gameOverWidget.Add(gameOverLabel);

            Button retreatButton = new();
            retreatButton.text = "Retreat?";
            retreatButton.style.color = Color.red;
            retreatButton.style.width = new Length(50, LengthUnit.Percent);
            gameOverWidget.Add(retreatButton);
            retreatButton.RegisterCallback<ClickEvent>(Retreat);
        }

        VisualElement coinWidget = new();
        uiDocument.rootVisualElement.Add(coinWidget);
        coinWidget.style.flexDirection = FlexDirection.Row;
        coinWidget.style.position = Position.Absolute;
        coinWidget.style.top = 0;
        coinWidget.style.right = 0;
        coinWidget.style.unityTextAlign = TextAnchor.MiddleRight;

        Image coinImage = new();
        coinImage.style.height = 15;
        coinImage.style.width = 15;
        coinImage.style.marginTop = coinImage.style.marginLeft = 3;
        coinImage.sprite = gameConfig.coinSprite;
        coinWidget.Add(coinImage);

        Label coinLabel = new();
        coinLabel.text = string.Format("{0}", player.GetMoney());
        coinLabel.style.color = Color.yellow;
        coinWidget.Add(coinLabel);

        VisualElement healthWidget = new();
        uiDocument.rootVisualElement.Add(healthWidget);
        healthWidget.style.flexDirection = FlexDirection.Row;
        healthWidget.style.position = Position.Absolute;
        healthWidget.style.top = 0;
        healthWidget.style.left = 0;

        for (int i = 0; i < player.GetHealth(); i++)
        {
            Image healthHeart = new();
            healthHeart.style.height = 15;
            healthHeart.style.width = 15;
            healthHeart.style.marginTop = healthHeart.style.marginLeft = 3;
            healthHeart.sprite = gameConfig.healthHeartSprite;
            healthWidget.Add(healthHeart);
        }
    }

    private void Retreat(ClickEvent evt)
    {
        Debug.Log("Retreating!");
        FindObjectOfType<LevelGenerator>().GenerateShip();
        ReDraw();
    }

    private void Erase()
    {
        Destroy(uiContainer);
    }
}