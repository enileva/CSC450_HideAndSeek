using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameUI : MonoBehaviour
{
    public TMP_Text healthText;   // Reference to the health text
    public TMP_Text coinageText;     // Reference to the coin text
    public TMP_Text timerText;    // Reference to the timer text
    public TMP_Text infoText;
    public Slider healthSlider;
    public Image healthFill;
    public GameObject gameOverPanel;
    public TMP_Text gameOverTitleText;
    public TMP_Text gameOverDetailText;
    public float maxHealth = 20;


    private float currentHealth;
    private int coins = 0;
    private float timer = 60f;
    private bool isTimerRunning = true;
    private bool isGameOver = false; private Vector3 deadZonePos = new Vector3(9999, -9999, 9999);


    void Start()
    {
        currentHealth = maxHealth; // Start with full health
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
            healthSlider.value = 1f;
        }
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        UpdateHealthUI();          // Initialize the health UI
        UpdateCoinsUI();           // Initialize the coins UI
        UpdateTimerUI();           // Initialize the timer UI
        //StartCoroutine(PulseText(timerText));
        StartCoroutine(PulseTimer());

    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer -= Time.deltaTime;  // Increase timer
            if (timer <= 0f)
            {
                timer = 0f;
                isTimerRunning = false;
                ShowFinalScore();
            }
            UpdateTimerUI();          // Update the timer UI
        }
    }
    void ShowFinalScore()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        infoText.text = string.Format("{0:00}:{1:00}", minutes, seconds) +
                        " - Time's up! You collected " + coins + " coins.";
    }


    public void TakeDamage(float amount)
    {
        if (isGameOver) return;
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHealthUI(); // Update health UI
        UpdateInfoHealthUI(amount);
        if (currentHealth <= 0f)
        {
            isTimerRunning = false;
            TriggerGameOver("You were caught!");

        }
    }

    void TriggerGameOver(string title)
    {
        isGameOver = true;

        // Show panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverTitleText != null)
            gameOverTitleText.text = title;

        if (gameOverDetailText != null)
            gameOverDetailText.text =
                "You collected " + coins + " coins.";

        // Stop player movement (local only)
        var controller = FindObjectOfType<FPSController>();
        if (controller != null && controller.isOwner)
        {
            controller.canMove = false;
            controller.gameObject.tag = "Untagged";
            controller.KillLocalPlayer();
        }

        // Unlock cursor so they can use UI / quit
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinsUI();  // Update coins UI
        UpdateInfoCoinUI(amount);
    }

    void UpdateHealthUI()
    {
        healthText.text = currentHealth.ToString("0") + "/" + maxHealth.ToString("0") + " h"; // Display health
        float percent = currentHealth / maxHealth;
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Clamp01(percent);
        }
        if (healthFill != null)
        {
            if (percent > 0.5f)
                healthFill.color = Color.green;                    // healthy
            else if (percent > 0.2f)
                healthFill.color = Color.yellow;
            else
                healthFill.color = Color.red;                      // danger
        }

        if (currentHealth > maxHealth / 2)
            healthText.color = Color.green;
        else if (currentHealth > maxHealth / 5)
        {
            healthText.color = Color.yellow;
        }
        else
            healthText.color = new Color(240f / 255f, 116f / 255f, 116f / 255f);
    }

    void UpdateCoinsUI()
    {
        coinageText.text = "$" + coins.ToString(); // Display coin count
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60); // Calculate minutes
        int seconds = Mathf.FloorToInt(timer % 60); // Calculate seconds
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Display timer

        if (minutes == 0 && seconds <= 30)
        {
            timerText.color = new Color(240f / 255f, 116f / 255f, 116f / 255f);
        }
    }

    void UpdateInfoCoinUI(float amount)
    {
        int minutes = Mathf.FloorToInt(timer / 60); // Calculate minutes
        int seconds = Mathf.FloorToInt(timer % 60); // Calculate seconds
        if (amount == 1)
        {
            infoText.text = string.Format("{0:00}:{1:00}", minutes, seconds) + " - Woohoo! You gained " + amount.ToString("0") + " coin!";
        }
        else
        {
            infoText.text = string.Format("{0:00}:{1:00}", minutes, seconds) + " - Woohoo! You gained " + amount.ToString("0") + " coins!";

        }
    }
    void UpdateInfoHealthUI(float amount)
    {
        int minutes = Mathf.FloorToInt(timer / 60); // Calculate minutes
        int seconds = Mathf.FloorToInt(timer % 60); // Calculate seconds
        infoText.text = string.Format("{0:00}:{1:00}", minutes, seconds) + " - Oh no! You lost " + amount.ToString("0") + " health!";
    }

    IEnumerator PulseTimer()
    {
        float originalSize = timerText.fontSize;
        float pulseAmount = 3f;      // How much bigger the text gets
        float pulseDuration = 0.3f;   // How long the pulse lasts

        while (timer > 0)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);

            // Only pulse when timer <= 30 seconds
            if (minutes == 0 && seconds <= 30)
            {
                // Grow
                float elapsed = 0f;
                while (elapsed < pulseDuration)
                {
                    timerText.fontSize = Mathf.Lerp(originalSize, originalSize + pulseAmount, elapsed / pulseDuration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                // Shrink
                elapsed = 0f;
                while (elapsed < pulseDuration)
                {
                    timerText.fontSize = Mathf.Lerp(originalSize + pulseAmount, originalSize, elapsed / pulseDuration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                // Wait until the next second tick
                yield return new WaitForSeconds(1f - 2 * pulseDuration);
            }
            else
            {
                // Ensure text returns to normal size when above 30 seconds
                timerText.fontSize = originalSize;
                yield return null;
            }
        }

        // Make sure text size resets at the end
        timerText.fontSize = originalSize;
    }
    public void OnQuitButtonPressed()
    {
        Debug.Log("Quit button pressed");

        // In a real build, quit the application
#if UNITY_EDITOR
        // Stop play mode in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
