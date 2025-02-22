using UnityEngine;
using System;

public class LadderButton : MonoBehaviour
{
    private bool pressed = false;
    public event Action<int> OnButtonPress;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {

            if (collision.gameObject.CompareTag("Player") && !pressed)
            {
                Pressed();
                pressed = true;
                
            }
        }
    }

    private void Pressed()
    {
        OnButtonPress?.Invoke(LevelManager.level);
    }
}
