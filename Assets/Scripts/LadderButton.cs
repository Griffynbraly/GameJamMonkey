using UnityEngine;
using System;

public class LadderButton : MonoBehaviour
{
    private bool pressed = false;
    public event Action<int> OnButtonPress;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {

            if (collision.gameObject.CompareTag("Player") && !pressed)
            {
                Pressed();
                pressed = true;
                Destroy(gameObject);
            }
        }
    }

    private void Pressed()
    {
        OnButtonPress?.Invoke(LevelManager.level);
    }
}
