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

    // End credit scenes
    public Image[] endCreditImages;  // End credit images
    private int currentEndCreditIndex = 0;  // Index for end credit images

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
            // Make sure we don't access out-of-bounds elements and show hearts only up to maxHealth
            if (i < maxHealth)
            {
                if (i < playerHealth)
                {
                    heartContainers[i].sprite = fullHeart;  // Set heart to full if health is > index
                }
                else
                {
                    heartContainers[i].sprite = emptyHeart;  // Set heart to empty if health is <= index
                }
                heartContainers[i].enabled = true;  // Show hearts up to maxHealth
            }
            else
            {
                heartContainers[i].enabled = false;  // Hide extra hearts beyond maxHealth
            }
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

    // Function to play the end game credits
    public void EndGame()
    {
        if (isCutsceneRunning) return;  // Prevent reactivation if a cutscene is running
        isCutsceneRunning = true;

        // Set the background and black background for the end credits
        background.SetActive(false);
        blackBackground.SetActive(true);
        StartCoroutine(FadeIn(blackBackgroundCanvasGroup));

        // Start the end credit scene
        cutscenePanel.SetActive(true);
        currentEndCreditIndex = 0;
        ShowEndCreditImage();
        
        StartCoroutine(PlayEndCreditSequence());
    }

    // Function to show each end credit image
    private void ShowEndCreditImage()
    {
        for (int i = 0; i < endCreditImages.Length; i++)
        {
            CanvasGroup canvasGroup = endCreditImages[i].GetComponent<CanvasGroup>();
            endCreditImages[i].gameObject.SetActive(i == currentEndCreditIndex);
            if (i == currentEndCreditIndex)
            {
                StartCoroutine(FadeIn(canvasGroup));
            }
        }
    }

    // Coroutine to play through the end credit sequence
    private IEnumerator PlayEndCreditSequence()
    {
        while (currentEndCreditIndex < endCreditImages.Length)
        {
            yield return new WaitForSeconds(fadeDuration + cutsceneDuration);  // Wait for the image duration
            CanvasGroup currentCanvasGroup = endCreditImages[currentEndCreditIndex].GetComponent<CanvasGroup>();
            yield return StartCoroutine(FadeOut(currentCanvasGroup));  // Fade out current image
            currentEndCreditIndex++;

            if (currentEndCreditIndex < endCreditImages.Length)
            {
                ShowEndCreditImage();  // Show next image
            }
        }

        // Wait for the final fade out and then complete the end game
        yield return new WaitForSeconds(fadeDuration);
        yield return StartCoroutine(FadeOut(blackBackgroundCanvasGroup));
        EndEndCreditScene();
    }

    // Function to finish the end credit scene
    private void EndEndCreditScene()
    {
        cutscenePanel.SetActive(false);
        blackBackground.SetActive(false);
        isCutsceneRunning = false;
    }
}
