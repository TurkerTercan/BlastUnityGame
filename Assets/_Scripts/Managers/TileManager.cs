using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviourSingleton<TileManager>
{

    //Conditions
    [SerializeField] private int numColor = 6;
    [SerializeField] private int rowCount = 10;
    [SerializeField] private int columnCount = 12;
    [SerializeField] private int[] conditions = new int[] {4, 7, 9};

    //Sprites
    [SerializeField] private Sprite[] blueColorSprites;
    [SerializeField] private Sprite[] greenColorSprites;
    [SerializeField] private Sprite[] pinkColorSprites;
    [SerializeField] private Sprite[] purpleColorSprites;
    [SerializeField] private Sprite[] redColorSprites;
    [SerializeField] private Sprite[] yellowColorSprites;

    //Prefabs
    [SerializeField] private Tile tilePrefab;

    //Helpers
    [SerializeField] private float xSize = 0.902f, ySize = 0.86f;

    private static GameObject tileContainer;
    private Tile[,] tiles;
    private Helper.TileColor[,] tileColors; 
    private Camera mainCamera;
    private Sprite[][] spriteContainer = new Sprite[6][];

    private bool[,] canBlast;

    private void Awake()
    {
        if (tileContainer == null)
            tileContainer = new GameObject("Tile Container");

        mainCamera = Camera.main;

        tiles = new Tile[columnCount, rowCount];
        tileColors = new Helper.TileColor[columnCount, rowCount];
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
                tiles[x, y].SetSprite(spriteContainer[randomColor][0], y + 2);
                tileColors[x, y] = (Helper.TileColor)randomColor;
                tiles[x, y].SetTileCoord(x, y);
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
                //Left Check 
                if (x != 0 && tileColors[x - 1, y] == tileColors[x, y])
                {
                    canBlast[x - 1, y] = true;
                    canBlast[x, y] = true;
                }

                //Right Check
                if(x != columnCount - 1 && tileColors[x + 1, y] == tileColors[x, y])
                {
                    canBlast[x + 1, y] = true;
                    canBlast[x, y] = true;
                }

                //Down Check
                if(y != 0 && tileColors[x, y - 1] == tileColors[x, y])
                {
                    canBlast[x, y - 1] = true;
                    canBlast[x, y] = true;
                }

                //Up Check
                if (y != rowCount - 1 && tileColors[x, y + 1] == tileColors[x, y])
                {
                    canBlast[x, y + 1] = true;
                    canBlast[x, y] = true;
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

                int x = tile.xCoord;
                int y = tile.yCoord;

                if (canBlast[x, y])
                    tile.gameObject.SetActive(false);

            }

        }
    }
}
