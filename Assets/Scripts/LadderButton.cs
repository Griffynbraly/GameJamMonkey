using UnityEngine;
using System;

public class LadderButton : MonoBehaviour
{
    private bool pressed = false;
    public event Action OnButtonPress;

    private void OnTriggerEnter2D(Collider2D collision)
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
        OnButtonPress?.Invoke();
    }
}
