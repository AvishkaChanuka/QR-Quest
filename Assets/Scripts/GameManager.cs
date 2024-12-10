using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GridWave { BLACKUP, BLACKDOWN };
    public GridWave waveStatus;

    public int countTurns = 0;
    public int wavePeriod = 8;
    private int _currentWaveTurns;

    private void Start()
    {
        waveStatus = GridWave.BLACKUP;
    }

    public int GetCurrentWaveTurns()
    {
        return countTurns % wavePeriod;
    }
}
