using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestChecker : MonoBehaviour
{

    public bool IsBothPlayersinArea = false;

    int playerCount = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerCount++;
        }

        if(playerCount == 2)
        {
            IsBothPlayersinArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerCount--;
            IsBothPlayersinArea = false;
        }
    }
}
