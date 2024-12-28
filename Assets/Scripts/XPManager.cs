using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set;}

    [SerializeField] private float xpGoalBase;
    [SerializeField] private Image xpBar;
    [SerializeField] private TextMeshProUGUI levelText;
    private float xpGoal;
    
    private float currXp;
    private int currLevel;

    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}
    }

    void Start() {
        currXp = 0;
        currLevel = 1;
        xpGoal = CalculateXpGoal();
        // xpGoal = 10f;
        SetXpBar();
    }

    public void AddXp(float newXp) {
        currXp += newXp;
        if (currXp >= xpGoal) {
            currXp = xpGoal - currXp;
            LevelUp();
        }
        SetXpBar();
    }

    private float CalculateXpGoal() {
        return xpGoalBase * Mathf.Pow(currLevel, 1.1f);
        // return xpGoalBase * Mathf.Pow(1.02f, currLevel-1);
        // return xpGoal * 1.02f;
    }

    private void LevelUp() {
        currLevel += 1;
        levelText.text = "LVL    -    " + currLevel;
        xpGoal = CalculateXpGoal();
        // CardManager.Instance.ShowCards();
    }

    private void SetXpBar() {
        xpBar.fillAmount = currXp / xpGoal;
    }

    public int GetPlayerLevel() {
        return currLevel;
    }
}
