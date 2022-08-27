using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviourSingleton<TileManager>
{

    //Conditions
    [Header("Conditions")]
    [SerializeField] private int numColor = 6;
    [SerializeField] private int rowCount = 10;
    [SerializeField] private int columnCount = 12;
    public int[] conditions = new int[] {4, 7, 9};

    //Sprites
    [Header("Sprites")]
    [SerializeField] private Sprite[] blueColorSprites;
    [SerializeField] private Sprite[] greenColorSprites;
    [SerializeField] private Sprite[] pinkColorSprites;
    [SerializeField] private Sprite[] purpleColorSprites;
    [SerializeField] private Sprite[] redColorSprites;
    [SerializeField] private Sprite[] yellowColorSprites;

    //Prefabs
    [Header("Prefabs")]
    [SerializeField] private Tile tilePrefab;

    //Helpers
    [Header("Helpers")]
    [SerializeField] private float xSize = 0.902f;
    [SerializeField] private float ySize = 0.86f;

    private static GameObject tileContainer;
    private static GameObject groupContainer;
    public Tile[,] tiles;
    private Camera mainCamera;
    public Sprite[][] spriteContainer = new Sprite[6][];

    private bool[,] canBlast;

    private void Awake()
    {
        if (tileContainer == null)
            tileContainer = new GameObject("Tile Container");
        if (groupContainer == null)
            groupContainer = new GameObject("Group Container");

        mainCamera = Camera.main;

        tiles = new Tile[columnCount, rowCount];
        canBlast = new bool[columnCount, rowCount];

        spriteContainer[0] = blueColorSprites;
        spriteContainer[1] = greenColorSprites;
        spriteContainer[2] = pinkColorSprites;
        spriteContainer[3] = purpleColorSprites;
        spriteContainer[4] = redColorSprites;
        spriteContainer[5] = yellowColorSprites;

    }

    void Start()
    {
        for (int y = 0; y < rowCount; y++)
        {
            for (int x = 0; x < columnCount; x++)
            {
                tiles[x, y] = Instantiate(tilePrefab, new Vector3(x * xSize, y * ySize, 0.0f), Quaternion.identity, tileContainer.transform);

                int randomColor = Random.Range(0, numColor);
                tiles[x, y].SetSpriteAndOrder(spriteContainer[randomColor][0], y + 2);
                tiles[x, y].SetGridCoordAndColor(x, y, (Helper.TileColor)randomColor);
            }
        }

        //Centeralize the camera to center of the tiles
        Vector3 middlePoint = (tiles[columnCount - 1, rowCount - 1].gameObject.transform.position + tiles[0, 0].gameObject.transform.position) / 2.0f;
        middlePoint.z = -10.0f;
        mainCamera.transform.position = middlePoint;


        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                if (canBlast[x, y])
                    continue;

                Tile left = null, right = null, up = null, down = null;
                //Left Check 
                if (x != 0 && tiles[x - 1, y].TileColor == tiles[x, y].TileColor)
                    left = tiles[x - 1, y];

                //Right Check
                if (x != columnCount - 1 && tiles[x + 1, y].TileColor == tiles[x, y].TileColor)
                    right = tiles[x + 1, y];

                //Down Check
                if (y != 0 && tiles[x, y - 1].TileColor == tiles[x, y].TileColor)
                    down = tiles[x, y - 1];

                //Up Check
                if (y != rowCount - 1 && tiles[x, y + 1].TileColor == tiles[x, y].TileColor)
                    up = tiles[x, y + 1];

                if (left || right || up || down)
                {
                    TileGroup tileGroup = null;
                    if (left)
                        tileGroup = left.CurrentGroup;
                    if (right && !tileGroup)
                        tileGroup = right.CurrentGroup;
                    if (up && !tileGroup)
                        tileGroup = up.CurrentGroup;
                    if (down && !tileGroup)
                        tileGroup = down.CurrentGroup;
                    if (!tileGroup)
                        tileGroup = groupContainer.AddComponent<TileGroup>();

                    if (left)
                    {
                        tileGroup.AddTile(left);
                        canBlast[x - 1, y] = true;
                    }
                    if (right)
                    {
                        tileGroup.AddTile(right);
                        canBlast[x + 1, y] = true;
                    }
                    if (up)
                    {
                        tileGroup.AddTile(up);
                        canBlast[x, y + 1] = true;
                    }
                    if (down)
                    {
                        tileGroup.AddTile(down);
                        canBlast[x, y - 1] = true;
                    }

                    canBlast[x, y] = true;
                    tileGroup.AddTile(tiles[x, y]);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"Clicked = {Input.mousePosition}");
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100);
            if (hit.collider != null)
            {
                Debug.Log($"{hit.collider.name}, {hit.collider.transform.position}");
                Tile tile = hit.collider.gameObject.GetComponent<Tile>();

                int xCoord = tile.XCoord;
                int yCoord = tile.YCoord;

                if (canBlast[xCoord, yCoord])
                {
                    TileGroup group = tile.CurrentGroup;
                    int minY = group.MinY;
                    int minX = -1, maxX = -1;

                    int[] countsByColumn = tile.CurrentGroup.GetTileGroup();
                    for (int i = 0; i < columnCount; i++)
                    {
                        if (minX == -1 && countsByColumn[i] != 0)
                            minX = i;

                        if (i != columnCount - 1 && countsByColumn[i] != 0 && countsByColumn[i + 1] == 0)
                            maxX = i;
                        else if (i == columnCount - 1 && countsByColumn[i] != 0)
                            maxX = i;
                    }
                    
                    for (int y = minY; y < rowCount; y++)
                    {
                        for (int x = minX; x <= maxX; x++)
                        {
                            canBlast[x, y] = false;
                            if (!tiles[x, y])
                            {
                                tiles[x, y].RemoveFromGroup();
                                if (y - countsByColumn[x] >= minY)
                                {
                                    tiles[x, y - countsByColumn[x]] = tiles[x, y];
                                    tiles[x, y] = null;
                                    tiles[x, y - countsByColumn[x]].SetGridCoord(x, y - countsByColumn[x]);
                                    tiles[x, y - countsByColumn[x]].gameObject.transform.position = new Vector3(x * xSize, (y - countsByColumn[x]) * ySize, 0);
                                }
                            }
                        }
                    }

                    Destroy(group);
                }
            }
        }
    }
}
