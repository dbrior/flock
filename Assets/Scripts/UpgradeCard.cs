using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public enum UpgradeType : int {
    RopeLength,
    ShearRadius,
    WateringRadius,
    Strength,
    Damage,
    Knockback,
    MoveSpeed,
    MaxHealth,
    Heal,
    PenCapacity,
    BlockChance,
    HealthRegen,
    CritChance,
    CritMultiplier
}

public class UpgradeCard : MonoBehaviour
{
    public UpgradeType upgradeType;
    public float value;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Image background;
    public Image icon;
    public Button button;
}