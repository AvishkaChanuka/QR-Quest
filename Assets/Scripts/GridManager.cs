using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    public Tile[] rowA, rowB, rowC, rowD, rowE, rowF, rowG, rowH, rowI, rowJ,
        rowK, rowL, rowM, rowN, rowO, rowP, rowQ, rowR, rowS, rowT, rowU;

    public Tile[][] grid;

    private GameManager gameManager;

    [SerializeField]
    private float minY = 0, maxY = 1;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
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
            MoveTile(minY);
        }
        else if(currentWaveTurn == 0)
        {
            MoveTile(maxY);
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

}
