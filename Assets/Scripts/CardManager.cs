using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CardRank
{
    public float rank;
    public Color color;
}

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }
    [SerializeField] private List<UpgradeCard> cards;
    [SerializeField] private GameObject cardMenu;
    [SerializeField] private List<CardRank> cardRanks;

    private List<UpgradeType> possibleUpgrades = new List<UpgradeType>{
        // UpgradeType.RopeLength,
        // UpgradeType.ShearRadius,
        // UpgradeType.WateringRadius,
        // UpgradeType.Strength,
        UpgradeType.Damage,
        UpgradeType.Knockback,
        // UpgradeType.MoveSpeed,
        UpgradeType.MaxHealth,
        UpgradeType.Heal,
        UpgradeType.BlockChance,
        UpgradeType.HealthRegen,
        UpgradeType.CritChance,
        UpgradeType.CritMultiplier
        // UpgradeType.PenCapacity
    };


    public Dictionary<UpgradeType,string> upgradeNames = new Dictionary<UpgradeType, string>{
        {UpgradeType.RopeLength, "ROPE   LENGTH"},
        {UpgradeType.ShearRadius, "SHEAR    RADIUS"},
        {UpgradeType.WateringRadius, "WATER    RADIUS"},
        {UpgradeType.Strength, "STRENGTH"},
        {UpgradeType.Damage, "DAMAGE"},
        {UpgradeType.Knockback, "KNOCKBACK"},
        {UpgradeType.MoveSpeed, "MOVE    SPEED"},
        {UpgradeType.MaxHealth, "MAX    HEALTH"},
        {UpgradeType.PenCapacity, "PEN    CAPACITY"},
        {UpgradeType.Heal, "HEAL"},
        {UpgradeType.BlockChance, "BLOCK    CHANCE"},
        {UpgradeType.HealthRegen, "HEALTH    REGEN"},
        {UpgradeType.CritChance, "CRIT    CHANCE"},
        {UpgradeType.CritMultiplier, "CRIT    MULTIPLIER"}
    };

    public Dictionary<UpgradeType,(float min,float max)> upgradeValueRanges = new Dictionary<UpgradeType, (float min,float max)>{
        {UpgradeType.RopeLength, (1f, 2f)},
        {UpgradeType.ShearRadius, (0.005f, 0.02f)},
        {UpgradeType.WateringRadius, (0.05f, 0.2f)},
        {UpgradeType.Strength, (0.2f, 1f)},
        {UpgradeType.Damage, (1f, 20f)},
        {UpgradeType.Knockback, (1f, 20f)},
        {UpgradeType.MoveSpeed, (0.1f, 0.25f)},
        {UpgradeType.MaxHealth, (1f, 20f)},
        {UpgradeType.PenCapacity, (1f, 10f)},
        {UpgradeType.Heal, (5f, 50f)},
        {UpgradeType.BlockChance, (1f, 5f)},
        {UpgradeType.HealthRegen, (5f, 25f)},
        {UpgradeType.CritChance, (0.5f, 10f)},
        {UpgradeType.CritMultiplier, (25f, 100f)}
    };

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}
    }

    public void ShowCards() {
        RandomizeCards();
        Time.timeScale = 0;
        cardMenu.SetActive(true);
        DeactivateButtons();
        StartCoroutine(DelayActivateButtons());
    }

    public void HideCards() {
        Time.timeScale = 1;
        cardMenu.SetActive(false);
    }
    
    public void SelectCard(UpgradeCard card) {
        GameObject[] playerObjs = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playerObj in playerObjs) {
            Player player = playerObj.GetComponent<Player>();
            player.AddUpgrade(card.upgradeType, card.value);
        }

        HideCards();
    }

    public void RandomizeCards() {
        // Ensure we get up to 4 distinct UpgradeTypes
        HashSet<UpgradeType> selectedUpgrades = new HashSet<UpgradeType>();

        while (selectedUpgrades.Count < 4) {
            UpgradeType randomUpgrade = possibleUpgrades[UnityEngine.Random.Range(0, possibleUpgrades.Count)];
            selectedUpgrades.Add(randomUpgrade); // HashSet ensures uniqueness
        }

        // Assign selected upgrades to the cards
        int index = 0;
        foreach (UpgradeCard card in cards) {
            UpgradeType type = selectedUpgrades.ElementAt(index); // Safe as HashSet maintains insertion order
            index++;

            (float min, float max) = upgradeValueRanges[type];
            float value = (float)Math.Round(UnityEngine.Random.Range(min, max), 1);
            float pctRank = (value - min) / (max - min);
            Color color = new Color(255f, 255f, 255f, 1f);

            for (int j = cardRanks.Count - 1; j >= 0; j--) {
                CardRank cardRank = cardRanks[j];
                if (pctRank >= cardRank.rank) {
                    color = cardRank.color;
                    break;
                }
            }

            card.background.color = color;
            card.value = value;
            card.upgradeType = type;
            card.description.text = "+" + value.ToString() + "    " + upgradeNames[type].ToString();
        }
    }


    private void DeactivateButtons() {
        foreach (UpgradeCard card in cards) {
            card.button.enabled = false;
        }
    }

    private void ActivateButtons() {
        foreach (UpgradeCard card in cards) {
            card.button.enabled = true;
        }
    }

    IEnumerator DelayActivateButtons() {
        yield return new WaitForSecondsRealtime(1f);
        ActivateButtons();
    }
}