using UnityEngine;
using System.Collections.Generic;

public class MoldBaseScript : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected List<Sprite> spriteList = new List<Sprite>();
    [SerializeField] BoxCollider2D boxCollider;

    public void SetScale(Vector2 newScale)
    {
        boxCollider.size = new Vector2(newScale.x, newScale.y);
        SetSprite(newScale);
    }

    public virtual void SetSprite(Vector2 spriteScale)
    {
        if (spriteScale == new Vector2(2, 3))
        {
            spriteRenderer.sprite = spriteList[0];
        }
        if (spriteScale == new Vector2(2, 2))
        {
            spriteRenderer.sprite = spriteList[1];
        }
        if (spriteScale == new Vector2(1, 3))
        {
            spriteRenderer.sprite = spriteList[2];
        }
        if (spriteScale == new Vector2(6, 1))
        {
            spriteRenderer.sprite = spriteList[3];
        }
        if (spriteScale == new Vector2(1, 2))
        {
            spriteRenderer.sprite = spriteList[4];
        }
    }
}
