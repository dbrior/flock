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


    public Dictionary<UpgradeType,string> upgradeNames = new Dictionary<UpgradeType, string>{
        {UpgradeType.RopeLength, "ROPE   LENGTH"},
        {UpgradeType.ShearRadius, "SHEAR    RADIUS"},
        {UpgradeType.WateringRadius, "WATER    RADIUS"},
        {UpgradeType.Strength, "STRENGTH"},
        {UpgradeType.Damage, "DAMAGE"},
        {UpgradeType.Knockback, "KNOCKBACK"},
        {UpgradeType.MoveSpeed, "MOVE    SPEED"},
        {UpgradeType.MaxHealth, "MAX    HEALTH"},
        {UpgradeType.PenCapacity, "PEN    CAPACITY"}
    };

    public Dictionary<UpgradeType,(float min,float max)> upgradeValueRanges = new Dictionary<UpgradeType, (float min,float max)>{
        {UpgradeType.RopeLength, (1f, 5f)},
        {UpgradeType.ShearRadius, (0.05f, 0.2f)},
        {UpgradeType.WateringRadius, (0.05f, 0.2f)},
        {UpgradeType.Strength, (0.2f, 1f)},
        {UpgradeType.Damage, (2f, 20f)},
        {UpgradeType.Knockback, (20f, 200f)},
        {UpgradeType.MoveSpeed, (0.2f, 0.5f)},
        {UpgradeType.MaxHealth, (2f, 10f)},
        {UpgradeType.PenCapacity, (1f, 10f)}
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
            UpgradeType type = (UpgradeType) UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(UpgradeType)).Length);

            (float min, float max) = upgradeValueRanges[type];
            float value = (float) Math.Round(UnityEngine.Random.Range(min, max), 1);
            float pctRank = (value - min) / (max - min);
            Color color = new Color (255f, 255f, 255f, 1f);

            for (int i = cardRanks.Count-1; i > 0; i--) {
                CardRank cardRank = cardRanks[i];
                if (pctRank >= cardRank.rank) {
                    color = cardRank.color;
                }
            }

            card.background.color = color;

            card.value = value;
            card.upgradeType = type;
            card.description.text = "+" + value.ToString() + "    " + upgradeNames[type].ToString();
        }
    }
}