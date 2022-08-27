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
    public int slidingAnims = 0;
    public int shuffleAnims = 0;
    private bool canPlay = true;
    private Vector3 centerPoint;

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
            for (int x = 0; x < columnCount; x++)
                CreateTile(x, y);

        //Centeralize the camera to center of the tiles
        centerPoint = (tiles[columnCount - 1, rowCount - 1].gameObject.transform.position + tiles[0, 0].gameObject.transform.position) / 2.0f;
        centerPoint.z = -10.0f;
        mainCamera.transform.position = centerPoint;
        centerPoint.z = 0.0f;

        CheckAndAddBlastingTiles(0, 0, columnCount, rowCount);
        
    }

    private void CreateTile(int x, int y)
    {
        tiles[x, y] = Instantiate(tilePrefab, new Vector3(x * xSize, y * ySize, 0.0f), Quaternion.identity, tileContainer.transform);

        int randomColor = Random.Range(0, numColor);
        tiles[x, y].SetSpriteAndOrder(spriteContainer[randomColor][0], y + 2);
        tiles[x, y].SetGridCoordAndColor(x, y, (Helper.TileColor)randomColor);
    }


    //Make sure all the grid of tiles has been initialized before using this function
    private void CheckAndAddBlastingTiles(int x_start, int y_start, int x_finish, int y_finish)
    {
        int count = 0;
        for (int x = x_start; x < x_finish; x++)
        {
            for (int y = y_start; y < y_finish; y++)
            {

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
                        count++;
                    }
                    if (right)
                    {
                        tileGroup.AddTile(right);
                        canBlast[x + 1, y] = true;
                        count++;
                    }
                    if (up)
                    {
                        tileGroup.AddTile(up);
                        canBlast[x, y + 1] = true;
                        count++;
                    }
                    if (down)
                    {
                        tileGroup.AddTile(down);
                        canBlast[x, y - 1] = true;
                        count++;
                    }

                    canBlast[x, y] = true;
                    tileGroup.AddTile(tiles[x, y]);
                    count++;
                }
            }
        }

        //There is no matching tile and grid needs to be shuffled
        if (count == 0)
        {
            StartShuffle();
        }
    }

    public void StartShuffle()
    {
        StartCoroutine(StartShuffleAnim());
    }

    private IEnumerator StartShuffleAnim()
    {
        Tile[,] shuffledTiles = new Tile[columnCount, rowCount];

        Debug.Log("Started");
        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                Tile currentTile = tiles[x, y];

                int newX, newY;
                do {
                    newX = Random.Range(0, columnCount);
                    newY = Random.Range(0, rowCount);
                } while (shuffledTiles[newX, newY] != null);

                shuffledTiles[newX, newY] = currentTile;
                if (currentTile.CurrentGroup != null)
                    currentTile.CurrentGroup.DisbandGroup();
                shuffleAnims++;
                currentTile.ShuffleAnim(centerPoint, newX, newY, new Vector3(newX * xSize, newY * ySize, 0.0f));

                yield return new WaitForSeconds(0.1f);
            }
        }


        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                tiles[x, y] = shuffledTiles[x, y];
            }
        }
        canBlast = new bool[columnCount, rowCount];

        yield return new WaitUntil(() => shuffleAnims == 0);
        CheckAndAddBlastingTiles(0, 0, columnCount, rowCount);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canPlay)
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
                    canPlay = false;
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

                    Debug.Log($"minY = {minY}, minX = {minX}, maxX = {maxX}");

                    group.DestroyTiles();
                    StartCoroutine(WaitDeathAnimation(group, minY, minX, maxX, countsByColumn));
                } else
                {
                    tiles[xCoord, yCoord].DenyAnimation();
                }
            }
        }
    }

    private IEnumerator WaitDeathAnimation(TileGroup group, int minY, int minX, int maxX, int[] countsByColumn)
    {
        Debug.Log("Death");
        yield return new WaitForSeconds(0.2f);
        Destroy(group);
        slidingAnims = 0;


        Debug.Log("Sliding");
        for (int y = minY; y < rowCount; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                canBlast[x, y] = false;
                if (tiles[x, y] != null)
                {
                    tiles[x, y].RemoveFromGroup();
                    if (y - countsByColumn[x] >= minY && tiles[x, y - countsByColumn[x]] == null)
                    {
                        tiles[x, y - countsByColumn[x]] = tiles[x, y];
                        tiles[x, y] = null;
                        tiles[x, y - countsByColumn[x]].SetGridCoord(x, y - countsByColumn[x]);
                        slidingAnims++;
                        tiles[x, y - countsByColumn[x]].SlidingAnimation(new Vector3(x * xSize, (y - countsByColumn[x]) * ySize, 0));

                    }
                }
            }
        }
        StartCoroutine(WaitSlidingAnim(countsByColumn));
    }


    private IEnumerator WaitSlidingAnim(int[] countsByColumn)
    {
        yield return new WaitUntil(() => slidingAnims == 0);

        Debug.Log("Drop anim");
        for (int x = 0; x < columnCount; x++)
        {
            int countAdd = countsByColumn[x];
            for (int i = 0; i < countAdd; i++)
            {
                CreateTile(x, rowCount - 1 - i);
                slidingAnims++;
                tiles[x, rowCount - 1 - i].DropAnimation(ySize);
            }
        }
        yield return new WaitUntil(() => slidingAnims == 0);

        Debug.Log("Group Check");
        for (int i = 0; i < columnCount; i++)
            for (int j = 0; j < rowCount; j++)
                canBlast[i, j] = false;
        CheckAndAddBlastingTiles(0, 0, columnCount, rowCount);
        canPlay = true;
    }
}
