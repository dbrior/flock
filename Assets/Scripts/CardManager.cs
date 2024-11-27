using System;
using System.Collections;
using System.Collections.Generic;
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
        UpgradeType.RopeLength,
        UpgradeType.ShearRadius,
        // UpgradeType.WateringRadius,
        UpgradeType.Strength,
        UpgradeType.Damage,
        UpgradeType.Knockback,
        UpgradeType.MoveSpeed,
        UpgradeType.MaxHealth,
        UpgradeType.Heal
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
        {UpgradeType.Heal, "HEAL"}
    };

    public Dictionary<UpgradeType,(float min,float max)> upgradeValueRanges = new Dictionary<UpgradeType, (float min,float max)>{
        {UpgradeType.RopeLength, (1f, 2f)},
        {UpgradeType.ShearRadius, (0.005f, 0.02f)},
        {UpgradeType.WateringRadius, (0.05f, 0.2f)},
        {UpgradeType.Strength, (0.2f, 1f)},
        {UpgradeType.Damage, (1f, 10f)},
        {UpgradeType.Knockback, (2f, 20f)},
        {UpgradeType.MoveSpeed, (0.1f, 0.25f)},
        {UpgradeType.MaxHealth, (2f, 10f)},
        {UpgradeType.PenCapacity, (1f, 10f)},
        {UpgradeType.Heal, (10f, 100f)}
    };

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}
    }

    public void ShowCards() {
        Time.timeScale = 0;
        cardMenu.SetActive(true);
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

        RandomizeCards();
        HideCards();
    }

    public void RandomizeCards() {
        foreach (UpgradeCard card in cards) {
            UpgradeType type = possibleUpgrades[UnityEngine.Random.Range(0, possibleUpgrades.Count)];

            (float min, float max) = upgradeValueRanges[type];
            float value = (float) Math.Round(UnityEngine.Random.Range(min, max), 1);
            float pctRank = (value - min) / (max - min);
            Color color = new Color (255f, 255f, 255f, 1f);

            for (int i = cardRanks.Count-1; i > 0; i--) {
                CardRank cardRank = cardRanks[i];
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
}