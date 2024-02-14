using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public GameConfig gameConfig;
    private GameObject uiContainer;

    private int curHealth;
    private int maxHealth;
    private int money;
    public Camera mainCamera;
    public Wizard player;
    public static Action NotifyRedraw;

    private void Awake()
    {
        NotifyRedraw += Draw;
    }

    private void Start()
    {
        Invoke(nameof(Draw), 0.2f);
    }

    public static void FindAndUpdateMoney(int money)
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (!uiManager)
        {
            Debug.LogError("FindAndUpdateMoney failed to find UIManager");
            return;
        }
        uiManager.UpdateMoney(money);
    }

    public void UpdateMoney(int money)
    {
        this.money = money;
        Draw();
    }

    public static void FindAndUpdateHealth(int curHealth, int maxHealth)
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (!uiManager)
        {
            Debug.LogError("FindAndUpdateHealth failed to find UIManager");
            return;
        }
        uiManager.UpdateHealth(curHealth, maxHealth);
    }

    public void UpdateHealth(int curHealth, int maxHealth)
    {
        this.curHealth = curHealth;
        this.maxHealth = maxHealth;
        Draw();
    }

    private void Draw()
    {
        if (uiContainer) Destroy(uiContainer);
        uiContainer = Utility.AttachChildObject(mainCamera.gameObject, "UIContainer");

        GameObject restartButtonContainer = Utility.AttachChildObject(uiContainer, "UIHealthText");
        UIDocument uiDocument = restartButtonContainer.AddComponent<UIDocument>();
        uiDocument.panelSettings = gameConfig.panelSettings;
        uiDocument.visualTreeAsset = gameConfig.uiAsset;

        if (curHealth == 0)
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
        coinLabel.text = string.Format("{0}", money);
        coinLabel.style.color = Color.yellow;
        coinWidget.Add(coinLabel);

        VisualElement healthWidget = new();
        uiDocument.rootVisualElement.Add(healthWidget);
        healthWidget.style.flexDirection = FlexDirection.Row;
        healthWidget.style.position = Position.Absolute;
        healthWidget.style.top = 0;
        healthWidget.style.left = 0;

        for (int i = 0; i < curHealth; i++)
        {
            Image healthHeart = new();
            healthHeart.style.height = 15;
            healthHeart.style.width = 15;
            healthHeart.style.marginTop = healthHeart.style.marginLeft = 3;
            healthHeart.sprite = gameConfig.healthHeartSprite;
            healthWidget.Add(healthHeart);
        }

        DrawSpellWidget(uiDocument);
    }

    private void DrawSpellWidget(UIDocument uiDocument)
    {
        if (!player) Debug.LogError("Missing player!");
        if (!player.spellInventory)
        {
            Debug.LogError("Player missing spellInventory");
            return;
        }

        VisualElement spellWidget = new();
        uiDocument.rootVisualElement.Add(spellWidget);
        spellWidget.style.flexDirection = FlexDirection.Row;
        spellWidget.style.position = Position.Absolute;
        spellWidget.style.bottom = 0;
        spellWidget.style.paddingBottom = 5f;
        spellWidget.style.left = 0;
        spellWidget.style.paddingLeft = 5f;

        for (int i = 0; i < player.spellInventory.GetSpellCount(); i++)
        {
            SpellData currentSpell = player.spellInventory.GetSpells()[i];
            Image spellImage = new();
            float spellSize = 15;
            if (player.spellInventory.GetCurrentSpell() == currentSpell) spellSize *= 1.5f;
            spellImage.style.height = spellSize;
            spellImage.style.width = spellSize;
            spellImage.style.paddingLeft = 5f;
            spellImage.style.paddingBottom = 5f;
            spellImage.sprite = currentSpell.GetSpellbookAnimation().frames[0];
            spellWidget.Add(spellImage);
        }
    }

    private void Retreat(ClickEvent evt)
    {
        Debug.Log("Retreating!");
        FindObjectOfType<LevelGenerator>().GenerateSummerHouse();
        Draw();
    }
}