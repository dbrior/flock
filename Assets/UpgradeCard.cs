using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    PenCapacity
}

public class UpgradeCard : MonoBehaviour
{
    public UpgradeType upgradeType;
    public float value;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
}