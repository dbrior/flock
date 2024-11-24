using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }
    [SerializeField] private List<UpgradeCard> cards;
    [SerializeField] private GameObject cardMenu;

    public Dictionary<UpgradeType,string> upgradeNames = new Dictionary<UpgradeType, string>{
        {UpgradeType.RopeLength, "ROPE   LENGTH"},
        {UpgradeType.ShearRadius, "SHEAR    RADIUS"},
        {UpgradeType.WateringRadius, "WATER    RADIUS"},
        {UpgradeType.Strength, "STRENGTH"},
        {UpgradeType.Damage, "DAMAGE"},
        {UpgradeType.Knockback, "Knockback"},
        {UpgradeType.MoveSpeed, "MOVE    SPEED"},
        {UpgradeType.PenCapacity, "PEN    CAPACITY"}
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
            float value = Random.Range(1, 20);
            UpgradeType type = (UpgradeType) Random.Range(0, System.Enum.GetValues(typeof(UpgradeType)).Length);

            card.value = value;
            card.upgradeType = type;
            card.description.text = "+" + value.ToString() + "    " + upgradeNames[type].ToString();
        }
    }
}