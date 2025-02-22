using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static int level { get; private set; }
    public bool buttonPressed = false;
    private LadderButton ladderButton;
    [SerializeField] SpawnManager spawnManager;
    void Start()
    {
        level = 0;
        spawnManager.LoadLevel(level);
        
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AssignButton();
        }
        
    }

    private void ButtonPressed( int level)
    {
        spawnManager.SpawnEndLadder(level);
    }

    public void AssignButton()
    {
        GameObject buttonObj = GameObject.FindGameObjectWithTag("LadderButton");

        StartCoroutine(AssignButtonAfterInstantiation(buttonObj));

    }

    private IEnumerator AssignButtonAfterInstantiation(GameObject buttonObj)
    {
        yield return null;  // Wait for the next frame to ensure the object is fully initialized
        LadderButton ladderButton = buttonObj.GetComponent<LadderButton>();
        if (ladderButton != null)
        {
            ladderButton.OnButtonPress += ButtonPressed;
        }
        else
        {
            Debug.LogWarning("LadderButton component not found on the button.");
        }
    }

    private void DesignButton()
    {
        ladderButton.OnButtonPress -= ButtonPressed;
    }
}
