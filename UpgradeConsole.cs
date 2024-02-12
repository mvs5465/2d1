using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeConsole : MonoBehaviour
{
    public GameConfig gameConfig;
    private GameObject uiContainer;
    private bool inRange = false;

    private bool healthPurchased = false;
    private bool maxHealthPurchased = false;
    private bool maxSpeedPurchased = false;

    public static GameObject Build(GameObject parent, GameConfig gameConfig)
    {
        GameObject ucObj = Utility.AttachChildObject(parent, "UpgradeConsoleContainer");
        ucObj.AddComponent<UpgradeConsole>().gameConfig = gameConfig;
        return ucObj;
    }
    private void Start()
    {
        gameObject.AddComponent<SpriteRenderer>().sprite = gameConfig.upgradeConsoleSprite;
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 1;
    }

    private void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            if (uiContainer)
            {
                Destroy(uiContainer);
                return;
            }
            DrawUI();
        }
    }

    private void DrawUI()
    {
        if (uiContainer) DestroyImmediate(uiContainer);
        uiContainer = Utility.AttachChildObject(gameObject, "UIContainer");
        UIDocument uiDocument = uiContainer.AddComponent<UIDocument>();
        uiDocument.visualTreeAsset = gameConfig.uiAsset;
        uiDocument.panelSettings = gameConfig.panelSettings;

        VisualElement upgradesContainer = new();
        uiDocument.rootVisualElement.Add(upgradesContainer);
        upgradesContainer.style.position = Position.Absolute;
        upgradesContainer.style.top = 20;
        upgradesContainer.style.left = 0;
        upgradesContainer.style.right = 0;
        upgradesContainer.style.backgroundColor = Color.white;
        upgradesContainer.style.alignItems = Align.Center;

        Label upgradeConsoleTitle = new();
        upgradeConsoleTitle.text = "Upgrades:";
        upgradesContainer.Add(upgradeConsoleTitle);

        Button healButton = AddButton(upgradesContainer, gameConfig.healthHeartSprite, "Heal", 0);
        if (!healthPurchased)
        {
            healButton.RegisterCallback<ClickEvent>(Heal);
        }
        else
        {
            healButton.SetEnabled(false);
        }

        Button maxHealthButton = AddButton(upgradesContainer, gameConfig.healthHeartSprite, "Add Heart", 100);
        if (!maxHealthPurchased && FindObjectOfType<Pigbro>().GetMoney() >= 100)
        {
            maxHealthButton.RegisterCallback<ClickEvent>(AddMaxHealth);
        }
        else
        {
            maxHealthButton.SetEnabled(false);
        }

        Button speedButton = AddButton(upgradesContainer, gameConfig.speedSprite, "Max Speed", 100);
        if (!maxSpeedPurchased && FindObjectOfType<Pigbro>().GetMoney() >= 100)
        {
            speedButton.RegisterCallback<ClickEvent>(AddMaxSpeed);
        }
        else
        {
            speedButton.SetEnabled(false);
        }
    }

    private void AddMaxSpeed(ClickEvent evt)
    {
        maxSpeedPurchased = true;
        Pigbro pigbro = FindObjectOfType<Pigbro>();
        pigbro.maxSpeed += 1;
        pigbro.GrantMoney(-100);
        DrawUI();
    }

    private Button AddButton(VisualElement buttonsContainer, Sprite itemSprite, string itemLabel, int itemCost)
    {
        Button upgradeButton = new();
        upgradeButton.style.width = new Length(50, LengthUnit.Percent);
        upgradeButton.style.height = 30;
        upgradeButton.style.justifyContent = Justify.SpaceBetween;
        upgradeButton.style.alignItems = Align.Center;
        upgradeButton.style.flexDirection = FlexDirection.Row;
        buttonsContainer.Add(upgradeButton);

        Image itemImage = new();
        itemImage.style.height = 15;
        itemImage.style.width = 15;
        itemImage.sprite = itemSprite;
        upgradeButton.Add(itemImage);

        Label buttonLabel = new();
        buttonLabel.text = itemLabel;
        buttonLabel.style.color = Color.yellow;
        upgradeButton.Add(buttonLabel);

        Image costImage = new();
        costImage.style.height = 15;
        costImage.style.width = 15;
        costImage.sprite = gameConfig.coinSprite;
        upgradeButton.Add(costImage);

        Label costLabel = new();
        costLabel.text = string.Format("{0}", itemCost);
        costLabel.style.color = Color.yellow;
        upgradeButton.Add(costLabel);

        return upgradeButton;
    }
    private void Heal(ClickEvent evt)
    {
        Pigbro pigbro = FindObjectOfType<Pigbro>();
        pigbro.Damage(-(pigbro.maxHealth - pigbro.GetHealth()));
        healthPurchased = true;
        DrawUI();
    }

    private void AddMaxHealth(ClickEvent evt)
    {
        Pigbro pigbro = FindObjectOfType<Pigbro>();
        pigbro.maxHealth += 1;
        pigbro.Damage(-1);
        pigbro.GrantMoney(-100);
        maxHealthPurchased = true;
        DrawUI();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Pigbro>())
        {
            inRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Pigbro>())
        {
            inRange = false;
            if (uiContainer) Destroy(uiContainer);
        }
    }
}