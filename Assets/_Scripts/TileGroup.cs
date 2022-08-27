using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGroup : MonoBehaviour
{
    private List<Tile> tileList = new List<Tile>();
    public Helper.TileColor GroupColor { get; private set; }

    public int MinY = int.MaxValue;
    private int[] countByColumn = new int[100];
    private int whichSprite = 0;
    private TileManager tileManager;

    private void Awake()
    {
        tileManager = TileManager.Instance;
        for (int i = 0; i < countByColumn.Length; i++)
            countByColumn[i] = 0;
    }

    public void AddTile(Tile newTile)
    {
        if (tileList.Contains(newTile))
            return;

        if (newTile.YCoord < MinY)
            MinY = newTile.YCoord;

        GroupColor = newTile.TileColor;
        tileList.Add(newTile);
        newTile.CurrentGroup = this;
        countByColumn[newTile.XCoord]++;

        if (tileList.Count > tileManager.conditions[2] && whichSprite != 3)
        {
            whichSprite = 3;
            UpdateAllTiles();
        } 
        else if (tileList.Count > tileManager.conditions[1] && whichSprite != 2)
        {
            whichSprite = 2;
            UpdateAllTiles();
        }
        else if (tileList.Count > tileManager.conditions[0] && whichSprite != 1)
        {
            whichSprite = 1;
            UpdateAllTiles();
        }

    }

    private void UpdateAllTiles()
    {
        foreach (var tile in tileList)
            tile.SetSprite(tileManager.spriteContainer[(int)GroupColor][whichSprite]);
    }

    public int[] GetTileGroup()
    {
        return countByColumn;
    }

    public void RemoveTile(Tile tile)
    {
        tileList.Remove(tile);
    }


    private void OnDestroy()
    {
        for (int i = tileList.Count - 1; i >= 0; i--)
        {
            Tile tile = tileList[i];
            tileList.Remove(tile);
            if (tile)
            {
                tileManager.tiles[tile.XCoord, tile.YCoord] = null;
                Destroy(tile.gameObject);
            }
        }
        Destroy(this, 1.0f);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
