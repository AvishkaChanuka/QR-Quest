using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    public Tile[] rowA, rowB, rowC, rowD, rowE, rowF, rowG, rowH, rowI, rowJ,
        rowK, rowL, rowM, rowN, rowO, rowP, rowQ, rowR, rowS, rowT, rowU;

    public Tile[][] grid;

    private GameManager gameManager;
    private CollectableManager collectableManager;

    [SerializeField]
    private float minY = 0, maxY = 1;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        collectableManager = FindAnyObjectByType<CollectableManager>();
        ChangeWave();
    }

    private void setUpGrid()
    {
        grid = new Tile[][]
        {
            rowA, rowB, rowC, rowD, rowE, rowF, rowG, rowH, rowI, rowJ,
            rowK, rowL, rowM, rowN, rowO, rowP, rowQ, rowR, rowS, rowT, rowU
        };
    }

    private void Awake()
    {
        setUpGrid();
    }

    private void Update()
    {
        
    }

    public void ChangeWave()
    {
        int currentWaveTurn = gameManager.GetCurrentWaveTurns();
        if(currentWaveTurn == gameManager.wavePeriod / 2)
        {
            gameManager.waveStatus = GameManager.GridWave.BLACKDOWN;
            MoveTile(minY);
            MovePlayer(minY+1);
        }
        else if(currentWaveTurn == 0)
        {
            gameManager.waveStatus= GameManager.GridWave.BLACKUP;
            MoveTile(maxY);
            MovePlayer(maxY+1);

            SpawnColectables();
        }
    }

    private void MoveTile(float yPos)
    {
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                if (grid[i][j].tileType == Tile.TileType.BLACK)
                {
                    Vector3 movePosition = grid[i][j].transform.position;
                    movePosition.y = yPos;
                    grid[i][j].transform.position = movePosition;
                }
            }
        }
    }

    private void MovePlayer(float yPos)
    {
        for(int i = 0; i< gameManager.players.Length; i++)
        {
            if (gameManager.players[i].GetPlayerPosition().tileType == Tile.TileType.BLACK)
            {
                Vector3 movePosition = gameManager.players[i].transform.position;
                movePosition.y = yPos;
                gameManager.players[i].transform.position = movePosition;
            }
        }
    }

    private void SpawnColectables()
    { 
        int count = 0;
        while (count < 3)
        {
            int randX = Random.Range(0, grid.Length);
            int randY = Random.Range(0, grid.Length);

            Tile tile = grid[randX][randY];

            if(tile.tileType == Tile.TileType.BLACK)
            {
                Vector3 spawnPos = new Vector3(tile.transform.position.x, maxY + 1, tile.transform.position.z);
                GameObject spawnObject = collectableManager.collectableObjects[count];
                Instantiate(spawnObject, spawnPos, Quaternion.identity);
                count++;
            }
            else
            {
                continue;
            }
        }
        
    }

}
