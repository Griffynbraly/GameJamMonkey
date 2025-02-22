using UnityEngine;
using System.Collections.Generic;

public class ClimbableScript : MoldBaseScript
{
   public override void SetSprite(Vector2 spriteScale)
    {
        if (spriteScale == new Vector2(2, 3))
        {
            spriteRenderer.sprite = spriteList[0];
        }
        if (spriteScale == new Vector2(2, 6))
        {
            spriteRenderer.sprite = spriteList[1];
        }
        if (spriteScale == new Vector2(2, 8))
        {
            spriteRenderer.sprite = spriteList[2];
        }
        if (spriteScale == new Vector2(2, 16))
        {
            spriteRenderer.sprite = spriteList[3];
        }
        if (spriteScale == new Vector2(1, 2))
        {
            spriteRenderer.sprite = spriteList[4];
        }
        if (spriteScale == new Vector2(1, 3))
        {
            spriteRenderer.sprite = spriteList[5];
        }
    }
}
