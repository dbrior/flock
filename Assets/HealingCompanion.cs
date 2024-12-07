using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingCompanion : MonoBehaviour
{
    private CharacterMover characterMover;
    private Damagable playerHealth;

    private bool enRoute = false;

    void Awake() {
        characterMover = GetComponent<CharacterMover>();
    }

    void Start() {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Damagable>();
    }

    public void ResetJob() {
        enRoute = false;
    }

    void Update()
    {
        if (!enRoute && playerHealth.GetHealthPct() < 0.75f) {
            enRoute = true;
            characterMover.NavigateTo(playerHealth.transform);
        }
    }
}
