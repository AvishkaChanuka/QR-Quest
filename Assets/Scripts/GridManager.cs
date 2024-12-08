using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    public Tile[] rowA, rowB, rowC, rowD, rowE, rowF, rowG, rowH, rowI, rowJ,
        rowK, rowL, rowM, rowN, rowO, rowP, rowQ, rowR, rowS, rowT, rowU;

    public Tile[][] grid;

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



}
