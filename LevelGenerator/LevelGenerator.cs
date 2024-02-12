using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    public GameConfig gameConfig;
    public LevelGeneratorData levelGeneratorData;
    public CaveData caveData;
    private GreenSwitch greenSwitch;
    private GameObject levelMapContainer;
    private Wizard wizard;

    private void Start()
    {
        wizard = FindObjectOfType<Wizard>();
        // Destroy(GameObject.Find("SummerHouse"));
        // GenerateSummerHouse();
    }

    public void GenerateLevel()
    {
        if (levelMapContainer) Destroy(levelMapContainer);
        levelMapContainer = Utility.AttachChildObject(gameObject, "LevelMapContainer");
        Grid grid = Utility.AttachChildObject(levelMapContainer, "GeneratedGrid").AddComponent<Grid>();
        GameObject tmgo = Utility.AttachChildObject(grid.gameObject, "GeneratedTilemap");
        tmgo.AddComponent<TilemapRenderer>();
        Tilemap tm = tmgo.GetComponent<Tilemap>();
        tmgo.AddComponent<TilemapCollider2D>();

        int xOffset = 0;
        DrawBox(new Vector3Int(xOffset, 0), levelGeneratorData.ChunkWidth, levelGeneratorData.ChunkHeight, tm);
        Vector3 playerSpawnPos = grid.GetCellCenterWorld(new Vector3Int(levelGeneratorData.ChunkWidth / 2, levelGeneratorData.ChunkHeight + 1));
        xOffset += levelGeneratorData.ChunkWidth;
        for (int i = 0; i < UnityEngine.Random.Range(levelGeneratorData.ChunksMin, levelGeneratorData.ChunksMax); i++)
        {
            int randHeight = UnityEngine.Random.Range(1, levelGeneratorData.ChunkHeight);
            DrawBox(new Vector3Int(xOffset, 0), levelGeneratorData.ChunkWidth, randHeight, tm);
            Vector3 enemyPos = grid.GetCellCenterWorld(new Vector3Int(xOffset + levelGeneratorData.ChunkWidth / 2, randHeight + 1));
            GameObject browner = Instantiate(levelGeneratorData.browner, enemyPos, Quaternion.identity);
            browner.transform.parent = levelMapContainer.transform;
            browner.transform.position = enemyPos;
            xOffset += levelGeneratorData.ChunkWidth;
        }
        DrawBox(new Vector3Int(xOffset, 0), levelGeneratorData.ChunkWidth, levelGeneratorData.ChunkHeight, tm);
        Vector3 greenSwitchPos = grid.GetCellCenterWorld(new Vector3Int(tm.cellBounds.max.x - 1, tm.cellBounds.max.y));
        greenSwitch = GreenSwitch.Build(levelMapContainer, gameConfig);
        greenSwitch.transform.position = greenSwitchPos;
        greenSwitch.levelGenerator = this;
        greenSwitch.mode = GreenSwitch.Mode.GENERATE_HIDEOUT;
        wizard.transform.position = playerSpawnPos;
        wizard.Reset();
    }

    public void GenerateShip()
    {
        if (levelMapContainer) Destroy(levelMapContainer);
        levelMapContainer = Utility.AttachChildObject(gameObject, "LevelMapContainer");
        Grid grid = Utility.AttachChildObject(levelMapContainer, "GeneratedGrid").AddComponent<Grid>();
        GameObject tmgo = Utility.AttachChildObject(grid.gameObject, "GeneratedTilemap");
        tmgo.AddComponent<TilemapRenderer>().sortingLayerName = "Background"; ;
        Tilemap tm = tmgo.GetComponent<Tilemap>();
        tmgo.AddComponent<TilemapCollider2D>();
        DrawRectangle(Vector3Int.zero, levelGeneratorData.HideoutWidth, levelGeneratorData.HideoutHeight, tm, levelGeneratorData.shipTile);
        Vector3 greenSwitchPos = grid.GetCellCenterWorld(new Vector3Int(tm.cellBounds.max.x - 2, 1));
        greenSwitch = GreenSwitch.Build(levelMapContainer, gameConfig);
        greenSwitch.transform.position = greenSwitchPos;
        greenSwitch.levelGenerator = this;
        greenSwitch.mode = GreenSwitch.Mode.GENERATE_LEVEL;

        GameObject backgo = Utility.AttachChildObject(grid.gameObject, "GeneratedTilemap");
        backgo.AddComponent<TilemapRenderer>().sortingLayerName = "Background";
        Tilemap backtm = backgo.GetComponent<Tilemap>();
        DrawBox(new Vector3Int(1, 1), levelGeneratorData.HideoutWidth - 1, levelGeneratorData.HideoutHeight + 1, backtm);

        Vector3 upgradeConsolePos = grid.GetCellCenterWorld(new Vector3Int(tm.cellBounds.max.x / 2, 1));
        UpgradeConsole.Build(levelMapContainer, gameConfig).transform.position = upgradeConsolePos - (Vector3.up * tm.cellSize.x / 2);

        Vector3 shipCenter = grid.GetCellCenterWorld(new Vector3Int(tm.cellBounds.max.x / 2, 3));
        wizard.transform.position = shipCenter;

        Light2D shipLight = Utility.AttachChildObject(levelMapContainer, "ShipLight").AddComponent<Light2D>();
        shipLight.lightType = Light2D.LightType.Freeform;
        UnityEngine.Debug.Log(string.Format(
            "min/max: ({0},{1})/({2},{3})",
            tm.cellBounds.min.x,
            tm.cellBounds.min.y,
            tm.cellBounds.max.x,
            tm.cellBounds.max.y
        ));
        Vector3[] path = new Vector3[]{
            tm.GetCellCenterWorld(tm.cellBounds.min),
            tm.GetCellCenterWorld(tm.cellBounds.max),
            // tm.GetCellCenterWorld(new Vector3Int(0,gameConfig.HideoutHeight - 1)),
            // tm.GetCellCenterWorld(new Vector3Int(gameConfig.HideoutWidth - 1,gameConfig.HideoutHeight - 1)),
            // tm.GetCellCenterWorld(new Vector3Int(gameConfig.HideoutWidth - 1,0)),

        };
        // shipLight.pointLightInnerRadius = 0;
        // shipLight.pointLightOuterRadius = 5;
        shipLight.SetShapePath(path);
        shipLight.intensity = 0.5f;
        shipLight.color = Color.blue + Color.white;
        shipLight.transform.position = shipCenter;

        wizard.Reset();
    }

    public void GenerateCave()
    {
        // config
        int maxHeight = 20;
        int caveChunkMinHeight = 4;
        int caveChunkMaxHeight = 8;
        int caveChunkMinWidth = 4;
        int caveChunkMaxWidth = 8;
        int minCaveLength = 4;
        int maxCaveLength = 10;
        int padding = 5;
        int caveLength = UnityEngine.Random.Range(minCaveLength, maxCaveLength);

        // setup
        Stopwatch sw = new();
        sw.Start();
        if (levelMapContainer) Destroy(levelMapContainer);
        levelMapContainer = Utility.AttachChildObject(gameObject, "LevelMapContainer");
        Grid grid = Utility.AttachChildObject(levelMapContainer, "GeneratedGrid").AddComponent<Grid>();
        GameObject tmgo = Utility.AttachChildObject(grid.gameObject, "GeneratedTilemap");
        tmgo.AddComponent<TilemapRenderer>();
        Tilemap tm = tmgo.GetComponent<Tilemap>();

        // Generate large box
        DrawBoxStrict(Vector3Int.zero, 5, maxHeight + caveChunkMaxHeight, tm, caveData.mm);

        int curX = tm.cellBounds.min.x;
        int curY = tm.cellBounds.max.y / 2;
        // Draw Caves
        for (int i = 0; i < caveLength; i++)
        {
            int boxWidth = Random.Range(caveChunkMinWidth, caveChunkMaxWidth);
            int boxHeight = Random.Range(caveChunkMinHeight, caveChunkMaxHeight);
            if (curX + boxWidth > tm.cellBounds.max.x)
            {
                DrawBoxStrict(new Vector3Int(curX, 0), boxWidth + 3, maxHeight + caveChunkMaxHeight, tm, caveData.mm);
            }
            DrawBoxStrict(new Vector3Int(curX, curY), boxWidth, boxHeight, tm, null);
            if (i == 0) wizard.transform.position = tm.CellToWorld(new Vector3Int(curX + boxWidth / 2, curY + boxHeight / 2));

            // Roll the dice to spawn enemy
            if (i > 0 && i < caveLength - 1)
            {
                int diceRoll = UnityEngine.Random.Range(0, 10);
                if (diceRoll > 4)
                {
                    GameObject browner = Instantiate(levelGeneratorData.browner, grid.GetCellCenterWorld(new Vector3Int(curX + boxWidth / 2, curY)), Quaternion.identity);
                    browner.transform.parent = levelMapContainer.transform;
                }
            }

            if (i == caveLength - 1)
            {
                Vector3 greenSwitchPos = grid.GetCellCenterWorld(new Vector3Int(curX + boxWidth / 2, curY));
                greenSwitch = GreenSwitch.Build(levelMapContainer, gameConfig);
                greenSwitch.transform.position = greenSwitchPos;
                greenSwitch.levelGenerator = this;
                greenSwitch.mode = GreenSwitch.Mode.GENERATE_HIDEOUT;
            }

            curX += boxWidth;
            curY += Random.Range(-2, 3);
        }
        // Add Padding
        // left/right
        DrawBoxStrict(new Vector3Int(tm.cellBounds.min.x - padding, tm.cellBounds.min.y), padding, tm.cellBounds.max.y, tm, caveData.mm);
        DrawBoxStrict(new Vector3Int(tm.cellBounds.max.x, tm.cellBounds.min.y), padding, tm.cellBounds.max.y, tm, caveData.mm);

        // bottom/top
        DrawBoxStrict(new Vector3Int(tm.cellBounds.min.x, tm.cellBounds.min.y - padding), tm.cellBounds.max.x + padding, padding, tm, caveData.mm);
        DrawBoxStrict(new Vector3Int(tm.cellBounds.min.x, tm.cellBounds.max.y), tm.cellBounds.max.x + padding, padding, tm, caveData.mm);

        // Decorate the cave
        DecorateCave(tm);

        // Cleanup
        tm.RefreshAllTiles();
        tmgo.AddComponent<TilemapCollider2D>();
        sw.Stop();
        UnityEngine.Debug.Log("Generated cave in " + sw.ElapsedMilliseconds + "ms");
    }

    private void DecorateCave(Tilemap tm)
    {
        int startX = tm.cellBounds.min.x + 1;
        int startY = tm.cellBounds.min.y + 1;
        // UnityEngine.Debug.Log("StartTileCode = " + GetTileCode(tm, new Vector3Int(startX, startY)));
        for (int x = startX; x < startX + tm.cellBounds.max.x - 2; x++)
        {
            for (int y = startY; y < startY + tm.cellBounds.max.y - 2; y++)
            {
                switch (GetTileCode(tm, new Vector3Int(x, y)))
                {
                    case "000111111":
                    case "001111111":
                    case "100111111":
                        tm.SetTile(new Vector3Int(x, y), caveData.um);
                        break;
                    case "000011111":
                    case "000011011":
                        tm.SetTile(new Vector3Int(x, y), caveData.ul);
                        break;
                    case "000110110":
                    case "000110111":
                        tm.SetTile(new Vector3Int(x, y), caveData.ur);
                        break;
                    case "110111111":
                        tm.SetTile(new Vector3Int(x, y), caveData.ul_r);
                        break;
                    case "011111111":
                        tm.SetTile(new Vector3Int(x, y), caveData.ur_r);
                        break;
                    case "111110110":
                    case "110110110":
                    case "110110111":
                        tm.SetTile(new Vector3Int(x, y), caveData.ml);
                        break;
                    case "111011011":
                    case "011011011":
                    case "011011111":
                        tm.SetTile(new Vector3Int(x, y), caveData.mr);
                        break;
                    case "111111000":
                    case "111111100":
                    case "111111001":
                        tm.SetTile(new Vector3Int(x, y), caveData.bm);
                        break;
                    case "111011000":
                    case "011011000":
                        tm.SetTile(new Vector3Int(x, y), caveData.bl);
                        break;
                    case "111111011":
                        tm.SetTile(new Vector3Int(x, y), caveData.bl_r);
                        break;
                    case "110110000":
                    case "111110000":
                        tm.SetTile(new Vector3Int(x, y), caveData.br);
                        break;
                    case "111111110":
                        tm.SetTile(new Vector3Int(x, y), caveData.br_r);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private string GetTileCode(Tilemap tm, Vector3Int tilePos)
    {
        StringBuilder sb = new("");
        for (int y = 1; y >= -1; y--)
        {
            for (int x = -1; x <= 1; x++)
            {
                string flag = tm.GetTile(tilePos + new Vector3Int(x, y)) ? "1" : "0";
                sb.Append(flag);
            }
        }
        return sb.ToString();
    }

    public void GenerateSummerHouse()
    {
        if (levelMapContainer) Destroy(levelMapContainer);
        levelMapContainer = Utility.AttachChildObject(gameObject, "LevelMapContainer");
        GameObject summerHouse = Instantiate(levelGeneratorData.summerHousePrefab, levelMapContainer.transform, false);
        wizard.transform.position = FindObjectOfType<PlayerSpawn>().GetPosition();
        wizard.Reset();
    }

    private void DrawBox(Vector3Int start, int width, int height, Tilemap tm)
    {
        for (int x = start.x; x < start.x + width; x++)
        {
            for (int y = start.y; y < start.y + height; y++)
            {
                Tile tile = levelGeneratorData.dirtMM;
                if (y == start.y + height - 1) tile = levelGeneratorData.dirtUM;
                if (x == start.x && y == start.y + height - 1 && !tm.GetTile(new Vector3Int(x - 1, y))) tile = levelGeneratorData.dirtUL;
                if (x == start.x && y == start.y + height - 1 && !tm.GetTile(new Vector3Int(x - 1, y))) tile = levelGeneratorData.dirtUL;
                if (x == start.x + width - 1 && y == start.y + height - 1) tile = levelGeneratorData.dirtUR;
                tm.SetTile(new Vector3Int(x, y), tile);
            }
        }
    }

    private void DrawBoxStrict(Vector3Int start, int width, int height, Tilemap tm, Tile tile)
    {
        for (int x = start.x; x < start.x + width; x++)
        {
            for (int y = start.y; y < start.y + height; y++)
            {
                tm.SetTile(new Vector3Int(x, y), tile);
            }
        }
    }

    private void DrawRectangle(Vector3Int start, int width, int height, Tilemap tm, Tile tile)
    {
        for (int x = start.x; x < start.x + width + 1; x++)
        {
            tm.SetTile(new Vector3Int(start.x + x, start.y), tile);
            tm.SetTile(new Vector3Int(start.x + x, start.y + height), tile);
        }
        for (int y = start.y; y < start.y + height; y++)
        {
            tm.SetTile(new Vector3Int(start.x, start.y + y), tile);
            tm.SetTile(new Vector3Int(start.x + width, start.y + y), tile);
        }
    }
}