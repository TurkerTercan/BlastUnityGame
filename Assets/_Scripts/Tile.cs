using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public int xCoord { get; private set; }
    public int yCoord { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void SetTileCoord(int x, int y)
    {
        xCoord = x;
        yCoord = y;
    }

    public void SetSprite(Sprite sprite, int soortingOrder)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = soortingOrder;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
