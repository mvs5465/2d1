using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    public GameObject browner;
    public Sprite pigLaserSprite;
    public Sprite healthbarSprite;
    public Sprite healthbarBackgroundSprite;
    public Sprite healthHeartSprite;
    public Sprite coinSprite;
    public Sprite upgradeConsoleSprite;
    public Sprite greenSwitchSprite;
    public Tile dirtTile;
    public Tile shipTile;
    public int ChunkWidth = 5;
    public int ChunkHeight = 4;
    public int ChunksMin = 5;
    public int ChunksMax = 9;
    public int HideoutWidth = 10;
    public int HideoutHeight = 7;
    public Font font;
    public VisualTreeAsset uiAsset;
    public PanelSettings panelSettings;
    public SpriteAnimatorData swordIdleAnimation;
    public SpriteAnimatorData swordAttackAnimation;
    public string playerSortingLayer;
    public string effectSortingLayer;
    public Sprite speedSprite;
    public SpriteAnimatorData throwingStarAnimation;
    public Tile shipBackTile;
}