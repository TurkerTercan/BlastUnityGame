using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public int XCoord { get; private set; }
    public int YCoord { get; private set; }
    public Helper.TileColor TileColor { get; private set; }
    public TileGroup CurrentGroup = null;

    [SerializeField] private float _speed = 1.0f;
    private Tween tween;

    private float _radius = 4.0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SlidingAnimation(Vector3 finalPos)
    {
        tween = transform.DOMove(finalPos, _speed).SetSpeedBased(true).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            TileManager.Instance.slidingAnims--;
        });
    }

    public void DropAnimation(float ySize)
    {
        Vector3 currentPos = transform.position;
        transform.position = currentPos + new Vector3(0, ySize * 3, 0);
        transform.DOMove(currentPos, _speed).SetSpeedBased(true).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            TileManager.Instance.slidingAnims--;
        });
    }

    public void DenyAnimation()
    {
        tween = transform.DOPunchRotation(Vector3.one, 0.2f).SetEase(Ease.OutBounce);
    }


    public Tween DeathAnim()
    {
        tween = transform.DOScale(transform.lossyScale * 1.2f, 0.2f).SetEase(Ease.OutBounce).OnComplete(() => Destroy(this.gameObject));
        return tween;
    }

    public void SetGridCoordAndColor(int x, int y, Helper.TileColor color)
    {
        XCoord = x;
        YCoord = y;
        TileColor = color;
        spriteRenderer.sortingOrder = y + 2;
    }

    public void SetGridCoord(int x, int y)
    {
        XCoord = x;
        YCoord = y;
        spriteRenderer.sortingOrder = y + 2;
    }

    public void RemoveFromGroup()
    {
        if (CurrentGroup != null)
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

    public void ShuffleAnim(Vector3 centerPoint, int newX, int newY, Vector3 finalPos)
    {
        Vector3[] waypoints = new Vector3[37];
        int index = 0;
        for (int i = 0; i <= 360; i+= 10)
        {
            waypoints[index++] = new Vector3(_radius * Mathf.Cos(i * Mathf.Deg2Rad), _radius * Mathf.Sin(i * Mathf.Deg2Rad)) + centerPoint;
        }
        waypoints[waypoints.Length - 1] = finalPos;

        transform.DOPath(waypoints, _speed * 2).SetSpeedBased(true).OnComplete(() =>
        {
            TileManager.Instance.shuffleAnims--;
            this.XCoord = newX;
            this.YCoord = newY;
            spriteRenderer.sortingOrder = newY + 2;
        });
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
