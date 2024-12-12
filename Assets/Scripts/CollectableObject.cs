using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    private GameManager gameManager;
    public enum ItemType { HEAL, ENERGY, COIN };
    public ItemType itemType;

    public int ItemValue = 1;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        if(gameManager.waveStatus == GameManager.GridWave.BLACKDOWN)
        {
            Destroy(gameObject);
        }
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}
