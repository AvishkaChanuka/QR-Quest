using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GridWave { BLACKUP, BLACKDOWN };
    public GridWave waveStatus;

    public int countTurns = 0;
    public int wavePeriod = 8;

    public Player[] players;

    public int currentPlayer = 0;

    private int _currentWaveTurns;

    private void Awake()
    {
        InitiatePalyers();
    }

    private void Start()
    {
        waveStatus = GridWave.BLACKUP;
    }

    public int GetCurrentWaveTurns()
    {
        return countTurns % wavePeriod;
    }

    private void InitiatePalyers()
    {
        for(int i = 0; i < players.Length; i++)
        {
            players[i].playerID = i;
        }
    }

    public void ChangePlayer(int playerNo)
    {
        currentPlayer = playerNo;
    }
}
