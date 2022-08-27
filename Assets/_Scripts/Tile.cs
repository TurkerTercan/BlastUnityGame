using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public int XCoord { get; private set; }
    public int YCoord { get; private set; }
    public Helper.TileColor TileColor { get; private set; }
    public TileGroup CurrentGroup = null;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetGridCoordAndColor(int x, int y, Helper.TileColor color)
    {
        XCoord = x;
        YCoord = y;
        TileColor = color;
    }

    public void SetGridCoord(int x, int y)
    {
        XCoord = x;
        YCoord = y;
    }

    public void RemoveFromGroup()
    {
        CurrentGroup.RemoveTile(this);
        CurrentGroup = null;
    }


    public void SetSpriteAndOrder(Sprite sprite, int soortingOrder)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = soortingOrder;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
