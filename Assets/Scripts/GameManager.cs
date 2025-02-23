using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int playerHealth = 5;
    public int maxHealth = 5;
    public Image[] heartContainers; // Heart container UI images
    public Sprite fullHeart;        // Sprite for full heart
    public Sprite emptyHeart;       // Sprite for empty heart

    // UI Elements
    public GameObject mainMenu;        // The entire main menu panel
    public Button playButton;          // The play button
    public Button quitButton;          // The quit button
    public GameObject cutscenePanel;   // Panel for cutscene images
    public Image[] cutsceneImages;     // Array of images for the cutscene
    public GameObject background;      // The background to disable during the cutscene
    private int currentCutsceneIndex = 0;

    // Black background for cutscenes
    public GameObject blackBackground; // The black background to fade out
    private CanvasGroup blackBackgroundCanvasGroup; // CanvasGroup for fading the black background

    // Time each image will display
    public float cutsceneDuration = 5f;    // Duration each cutscene image stays on screen
    public float fadeDuration = 1f;        // Time for each fade effect
    private bool isCutsceneRunning = false; // Lock variable to prevent reactivation during cutscenes

    private void Start()
    {
        blackBackgroundCanvasGroup = blackBackground.GetComponent<CanvasGroup>();
        playButton.onClick.AddListener(StartCutscene);
        quitButton.onClick.AddListener(QuitGame);
        ShowMainMenu();
        UpdateHealthUI(); // Initialize heart containers
    }

    private void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        cutscenePanel.SetActive(false);
        background.SetActive(true);
    }

    public void StartCutscene()
    {
        if (isCutsceneRunning) return;
        isCutsceneRunning = true;
        background.SetActive(false);
        blackBackground.SetActive(true);
        StartCoroutine(FadeIn(blackBackgroundCanvasGroup));
        cutscenePanel.SetActive(true);
        currentCutsceneIndex = 0;
        ShowCutsceneImage();
        StartCoroutine(PlayCutsceneSequence());
    }

    private void ShowCutsceneImage()
    {
        for (int i = 0; i < cutsceneImages.Length; i++)
        {
            CanvasGroup canvasGroup = cutsceneImages[i].GetComponent<CanvasGroup>();
            cutsceneImages[i].gameObject.SetActive(i == currentCutsceneIndex);
            if (i == currentCutsceneIndex)
            {
                StartCoroutine(FadeIn(canvasGroup));
            }
        }
    }

    private IEnumerator PlayCutsceneSequence()
    {
        while (currentCutsceneIndex < cutsceneImages.Length)
        {
            yield return new WaitForSeconds(fadeDuration + cutsceneDuration);
            CanvasGroup currentCanvasGroup = cutsceneImages[currentCutsceneIndex].GetComponent<CanvasGroup>();
            yield return StartCoroutine(FadeOut(currentCanvasGroup));
            currentCutsceneIndex++;
            if (currentCutsceneIndex < cutsceneImages.Length)
            {
                ShowCutsceneImage();
            }
        }
        yield return new WaitForSeconds(fadeDuration);
        yield return StartCoroutine(FadeOut(blackBackgroundCanvasGroup));
        EndCutscene();
    }

    private void EndCutscene()
    {
        cutscenePanel.SetActive(false);
        blackBackground.SetActive(false);
        isCutsceneRunning = false;
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    // This method is called when the player takes damage
    public void TakeDamage(int damage)
    {
        playerHealth = Mathf.Max(0, playerHealth - damage);  // Ensure health doesn't go below 0
        UpdateHealthUI();  // Update health UI after damage
        if (playerHealth <= 0)
        {
            Debug.Log("Player Died");
        }
    }

    // This method updates the health UI based on current player health
    private void UpdateHealthUI()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < playerHealth)
            {
                heartContainers[i].sprite = fullHeart;  // Set heart to full if health is > index
            }
            else
            {
                heartContainers[i].sprite = emptyHeart;  // Set heart to empty if health is <= index
            }
            heartContainers[i].enabled = i < maxHealth;  // Only show hearts up to maxHealth
        }
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(cutsceneDuration);  // Wait for cutscene duration before fading out
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
