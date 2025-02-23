using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] int playerHealth = 5;
    [SerializeField] int maxHealth = 5;
    [SerializeField] Image[] heartContainers; // Heart container UI images
    [SerializeField] Sprite fullHeart;        // Sprite for full heart
    [SerializeField] Sprite emptyHeart;

    [SerializeField] GameObject healthUI; 

    // UI Elements
    [SerializeField] GameObject mainMenu;        // The entire main menu panel
    [SerializeField] Button playButton;          // The play button
    [SerializeField] Button quitButton;          // The quit button
    [SerializeField] GameObject cutscenePanel;   // Panel for cutscene images
    [SerializeField] Image[] cutsceneImages;     // Array of images for the cutscene
    [SerializeField] GameObject background;      // The background to disable during the cutscene
    private int currentCutsceneIndex = 0;

    // Black background for cutscenes
    [SerializeField] GameObject blackBackground; // The black background to fade out
    private CanvasGroup blackBackgroundCanvasGroup; // CanvasGroup for fading the black background

    // Time each image will display
    [SerializeField] float cutsceneDuration = 0.5f;    // Duration each cutscene image stays on screen
    [SerializeField] float fadeDuration = 1f;        // Time for each fade effect
    private bool isCutsceneRunning = false; // Lock variable to prevent reactivation during cutscenes

    // End credit scenes
    [SerializeField] GameObject skipButton;
    [SerializeField] Image[] endCreditImages;  // End credit images
    [SerializeField] GameObject restartScreen;
    private int currentEndCreditIndex = 0;  // Index for end credit images

    static public event Action OnStartGame;
    static public event Action OnGameOver;
    private void Start()
    {
        restartScreen.SetActive(false);
        PlayerMove.OnPlayerDamaged += TakeDamage;
        PlayerMove.OnWheelTurned += EndGame;
        blackBackgroundCanvasGroup = blackBackground.GetComponent<CanvasGroup>();
        playButton.onClick.AddListener(StartCutscene);
        quitButton.onClick.AddListener(QuitGame);
        ShowMainMenu();
        UpdateHealthUI(); // Initialize heart containers
    }

   
    private void ShowMainMenu()
    {
        restartScreen.SetActive(false);
        mainMenu.SetActive(true);
        cutscenePanel.SetActive(false);
        background.SetActive(true);
    }

    public void StartCutscene()
    {
        if (isCutsceneRunning) return;
        skipButton.SetActive(true);
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

    public void EndCutscene()
    {
        cutscenePanel.SetActive(false);
        blackBackground.SetActive(false);
        isCutsceneRunning = false;
        healthUI.SetActive(true);
        skipButton.SetActive(false);
        StartGame();
    }
    private void QuitGame()
    {
        Application.Quit();
    }

    // This method is called when the player takes damage
    public void TakeDamage()
    {
        playerHealth = Mathf.Max(0, playerHealth - 1);  // Ensure health doesn't go below 0
        UpdateHealthUI();  // Update health UI after damage
        if (playerHealth <= 0)
        {
            EndGame();
            OnGameOver?.Invoke();
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
        healthUI.SetActive(false);
        background.SetActive(false);
        blackBackground.SetActive(true);
        StartCoroutine(FadeIn(blackBackgroundCanvasGroup));

        // Start the end credit scene
        //cutscenePanel.SetActive(true);
        currentEndCreditIndex = 0;
        if (playerHealth != 0)
        {
            ShowEndCreditImage();

            StartCoroutine(PlayEndCreditSequence());
        }
        else
        {
            restartScreen.SetActive(true);
        }
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
    public void EndEndCreditScene()
    {
        cutscenePanel.SetActive(false);
        blackBackground.SetActive(false);
        isCutsceneRunning = false;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void StartGame()
    {
        OnStartGame?.Invoke();
    }
    private void OnDisable()
    {
        PlayerMove.OnPlayerDamaged -= TakeDamage;
        PlayerMove.OnWheelTurned -= EndCutscene;
    }
}
