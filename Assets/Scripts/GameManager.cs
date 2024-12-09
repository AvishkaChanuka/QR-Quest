using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int countTurns = 0;
    public int wavePeriod = 8;
    private int _currentWaveTurns;

    public int GetCurrentWaveTurns()
    {
        return countTurns % wavePeriod;
    }
}
