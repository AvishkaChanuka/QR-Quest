using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    [SerializeField]
    private Slider healthUI;

    [SerializeField]
    private int currentHealth = 6, maxHealth = 6;

    public int chestValue = 100;

    private void Start()
    {
        healthUI.maxValue = maxHealth;
        healthUI.value = currentHealth;
    }

    public void GetAttack(int value)
    {
        currentHealth -= value;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthUI.value = currentHealth;
    }

    public bool IsChestClaimed()
    {
        if(currentHealth <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
