using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GridWave { BLACKUP, BLACKDOWN };
    public GridWave waveStatus;

    public int countTurns = 0;
    public int wavePeriod = 8;

    public Player[] players;

    public Chest[] chests;

    public int currentPlayer = 0;

    public bool isPlayerTurn = true;

    public GameObject nextButton, GameWinUI, GameOverUI;

    [SerializeField]
    private TextMeshProUGUI player1Coin, player2Coin, winnertxt;

    public int noOfChests = 3;

    private int _currentWaveTurns;

    private GridManager _gridManager;


    private void Awake()
    {
        nextButton.SetActive(false);
        InitiatePalyers();
    }

    private void Start()
    {
        waveStatus = GridWave.BLACKUP;
        _gridManager = FindAnyObjectByType<GridManager>();
    }
    private void Update()
    {
        GameOver();
        GameWin();
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
        nextButton.SetActive(true);
    }

    public void StartNextPlayerTurn()
    {
        countTurns++;
        _gridManager.ChangeWave();
        isPlayerTurn = true;
        nextButton.SetActive(false);
    }

    public void ChangePlayer(int playerNo)
    {
        currentPlayer = playerNo;
    }

    public void GameWin()
    {
        if(noOfChests <= 0)
        {
            string winner;
            if (players[0].coinLevel < players[1].coinLevel)
            {
                winner = players[1].name;
            }
            else
            {
                winner = players[0].name;
            }
            player1Coin.text = players[0].coinLevel.ToString();
            player2Coin.text = players[1].coinLevel.ToString();

            winnertxt.text = " Winner is " + winner;

            GameWinUI.SetActive(true );
            players[0].PlaySound(2);
        }
    }

    public void GameOver()
    {
        foreach(var player in players)
        {
            if(player.isAlive == false)
            {
                GameOverUI.SetActive(true) ;
                player.PlaySound(3);
                break;
            }
        }
    }

    public void GoHome()
    {
        SceneManager.LoadScene(1);
    }
}
