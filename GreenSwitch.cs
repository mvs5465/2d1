using UnityEngine;

public class GreenSwitch : MonoBehaviour
{
    public enum Mode
    {
        GENERATE_LEVEL,
        GENERATE_HIDEOUT
    }
    public LevelGenerator levelGenerator;

    private bool inRange = false;
    public Mode mode = Mode.GENERATE_LEVEL;
    private GameConfig gameConfig;

    public static GreenSwitch Build(GameObject parent, GameConfig gameConfig)
    {
        GameObject switchContainer = Utility.AttachChildObject(parent);
        GreenSwitch greenSwitch = switchContainer.AddComponent<GreenSwitch>();
        greenSwitch.gameConfig = gameConfig;
        return greenSwitch;
    }
    private void Start()
    {
        gameObject.AddComponent<SpriteRenderer>().sprite = gameConfig.greenSwitchSprite;
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = 1.5f;
        collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Wizard>())
        {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Wizard>())
        {
            inRange = false;
        }
    }

    private void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            switch (mode)
            {
                case Mode.GENERATE_LEVEL:
                    levelGenerator.GenerateLevel();
                    break;
                case Mode.GENERATE_HIDEOUT:
                default:
                    levelGenerator.GenerateSummerHouse();
                    break;
            }
        }
    }
}