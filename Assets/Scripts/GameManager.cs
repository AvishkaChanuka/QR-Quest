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

    public bool isPlayerTurn = true;

    private int _currentWaveTurns;

    private GridManager _gridManager;

    private void Awake()
    {
        InitiatePalyers();
    }

    private void Start()
    {
        waveStatus = GridWave.BLACKUP;
        _gridManager = FindAnyObjectByType<GridManager>();
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

    public void EndTurn()
    {
        ChangePlayer((currentPlayer + 1) % players.Length);
        isPlayerTurn = false;
        Invoke("StartNextPlayerTurn", 1f);
    }

    private void StartNextPlayerTurn()
    {
        countTurns++;
        _gridManager.ChangeWave();
        isPlayerTurn = true;
    }

    public void ChangePlayer(int playerNo)
    {
        currentPlayer = playerNo;
    }
}
