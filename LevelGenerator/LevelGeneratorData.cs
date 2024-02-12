using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(menuName = "ScriptableObjects/LevelGeneratorData")]
public class LevelGeneratorData : ScriptableObject
{
    public GameObject summerHousePrefab;
    public GameObject browner;
    public Tile dirtUL;
    public Tile dirtUM;
    public Tile dirtUR;
    public Tile dirtMM;
    public Tile dirtTile;
    public Tile shipTile;
    public Tile shipBackTile;
    public int ChunkWidth = 5;
    public int ChunkHeight = 4;
    public int ChunksMin = 5;
    public int ChunksMax = 9;
    public int HideoutWidth = 10;
    public int HideoutHeight = 7;
}