using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType { BLACK, WHITE };
    public TileType tileType;

    public enum TilePosition { A = 1, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U };
    public TilePosition positionX, positionY;

    [SerializeField]
    private ColorManager colorManager;

    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        ResetColor();
    }

    public void ChangeColor(int colorNo)
    {
        meshRenderer.materials[0].color = colorManager.middleColors[colorNo];
    }

    public void ResetColor()
    {
        if (tileType == TileType.BLACK)
        {
            meshRenderer.materials[0].color = colorManager.darkBaseColor;
            meshRenderer.materials[1].color = colorManager.darkBaseColor;
        }
        else
        {
            meshRenderer.materials[0].color = colorManager.lightBaseColor;
            meshRenderer.materials[1].color = colorManager.lightBaseColor;
        }
    }
}
