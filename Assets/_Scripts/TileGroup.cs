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
        newTile.SetSprite(tileManager.spriteContainer[(int)GroupColor][whichSprite]); ;

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
        tile.SetSprite(tileManager.spriteContainer[(int)GroupColor][0]);
        countByColumn[tile.XCoord]--;

        //If only one left in the group, disband the group
        if (tileList.Count == 1 && tileList[0] != null)
        {
            Tile _tile = tileList[0];
            _tile.CurrentGroup = null;
            _tile.SetSprite(tileManager.spriteContainer[(int)GroupColor][0]);
            tileList.Remove(_tile);
        }

        if (tileList.Count < tileManager.conditions[2] && whichSprite == 3)
        {
            whichSprite = 2;
            UpdateAllTiles();
        }
        else if (tileList.Count < tileManager.conditions[1] && whichSprite == 2)
        {
            whichSprite = 1;
            UpdateAllTiles();
        }
        else if (tileList.Count < tileManager.conditions[0] && whichSprite == 1)
        {
            whichSprite = 0;
            UpdateAllTiles();
        }

    }

    public void DisbandGroup()
    {
        foreach (var tile in tileList)
        {
            tile.CurrentGroup = null;
            tile.SetSprite(tileManager.spriteContainer[(int)GroupColor][0]);
        }

        tileList.Clear();
        Destroy(this);
    }

    private void OnDrawGizmos()
    {
        switch ((int)GroupColor)
        {
            case 0: Gizmos.color = Color.blue; break;
            case 1: Gizmos.color = Color.green; break;
            case 2: Gizmos.color = Color.grey; break;
            case 3: Gizmos.color = Color.cyan; break;
            case 4: Gizmos.color = Color.red; break;
            case 5: Gizmos.color = Color.yellow; break;
        }
        foreach (Tile tile in tileList)
            if (tile != null)
                Gizmos.DrawCube(tile.gameObject.transform.position, tile.gameObject.transform.lossyScale);
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

    public void DestroyTiles()
    {
        for (int i = tileList.Count - 1; i >= 0; i--)
        {
            Tile tile = tileList[i];
            tileList.Remove(tile);
            if (tile)
            {
                tileManager.tiles[tile.XCoord, tile.YCoord] = null;
                tile.DeathAnim();
            }
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
