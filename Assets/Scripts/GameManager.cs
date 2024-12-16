using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TextMeshProUGUI prestigeUI;

    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private TextMeshProUGUI pausePrestige;
    public bool isPaused {get; private set;}
    
    private int enemyKills;
    private int bossKills;


    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}

        bossKills = 0;
        enemyKills = 0;
    }

    public void BossKill() {
        bossKills += 1;
    }

    public void EnemyKill() {
        enemyKills += 1;
    }

    public void Retry() {
        SceneManager.LoadScene("SampleScene");
    }

    public void GameOver() {
        gameOverScreen.SetActive(true);
        gameOverScreen.GetComponent<UIFade>().StartFade();
        MusicManager.Instance.GameOver();

        int earnedPrestigePoints = CalculateEarnedPrestigePoints();
        int currentPrestigePoints = PlayerPrefs.GetInt("PrestigePoints", 0);
        PlayerPrefs.SetInt("PrestigePoints", currentPrestigePoints + earnedPrestigePoints);

        prestigeUI.text = "+" + earnedPrestigePoints.ToString();
    }

    public void ExitToMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public int CalculateEarnedPrestigePoints() {
        int playerLevel = XPManager.Instance.GetPlayerLevel();
        int daysSurvived = WaveManager.Instance.GetCurrentDay();

        int earnedPrestigePoints = ((playerLevel-1)*2) + ((daysSurvived-1)*10) + (bossKills*100) + (enemyKills/10);
        return earnedPrestigePoints;
    }

    public void PauseGame() {
        isPaused = true;
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        int earnedPrestigePoints = CalculateEarnedPrestigePoints();
        int currentPrestigePoints = PlayerPrefs.GetInt("PrestigePoints", 0);
        PlayerPrefs.SetInt("PrestigePoints", currentPrestigePoints + earnedPrestigePoints);

        pausePrestige.text = "+" + earnedPrestigePoints.ToString();
    }

    public void UnpauseGame() {
        isPaused = false;
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
    }

    // PlayerPref conevntions:
    // - Stored as a mutlipler to some base stat in game
    // - Key formatted as: {Character Name}-{Stat}
    // - There will be an extra entry {Character Name}-{Stat}-PurchaseCount
    // e.g. Knight-MaxHealth = 2 , would mean Knights start with double base health
    //
    // Prestige points stored under PrestigePoints
}
