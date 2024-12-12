using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    
    private int enemyKills;
    private int bossKills;

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}
    }

    public void ExitToMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void CalculatePrestigePoints() {
        int playerLevel = XPManager.Instance.GetPlayerLevel();

    }

    // PlayerPref conevntions:
    // - Stored as a mutlipler to some base stat in game
    // - Key formatted as: {Character Name}-{Stat}
    // - There will be an extra entry {Character Name}-{Stat}-PurchaseCount
    // e.g. Knight-MaxHealth = 2 , would mean Knights start with double base health
    //
    // Prestige points stored under PrestigePoints
}
