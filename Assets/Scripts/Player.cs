using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    const string IS_IDLE = "isIdle";
    const string IS_WALKING = "isWalking";
    const string IS_RUNNING = "isRunning";
    const string IS_ATTACKING = "isAttacking";
    const string IS_PICKUP = "isPickup";
    const string IS_HITTING = "isHitting";
    const string IS_DEAD = "isDead";
    public enum PlayerStatus { IDLE, WALKING, RUNNING, ATTACKING, PICKUP, HITTING, DEAD}
    public PlayerStatus playerStatus;

    Animator playerAnimator;
    GridManager gridManager;

    [SerializeField]
    LayerMask layerMask;

    private void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();

        playerStatus = PlayerStatus.IDLE;
        playerAnimator = GetComponent<Animator>();
        ChangeAnimation();
    }

    private void Update()
    {
        ChangeAnimation();

        AnalizeMovablePath();

        DoAction();
    }

    private void ChangeAnimation()
    {
        playerAnimator.SetBool(IS_IDLE, false);
        playerAnimator.SetBool(IS_WALKING, false);
        playerAnimator.SetBool(IS_RUNNING, false);
        playerAnimator.SetBool(IS_ATTACKING, false);
        playerAnimator.SetBool(IS_PICKUP, false);
        playerAnimator.SetBool(IS_HITTING, false);

        switch (playerStatus)
        {
            case PlayerStatus.IDLE:
                {
                    playerAnimator.SetBool(IS_IDLE, true);
                    break;
                }
            case PlayerStatus.WALKING:
                {
                    playerAnimator.SetBool(IS_WALKING,true);
                    break;
                }
            case PlayerStatus.RUNNING:
                {
                    playerAnimator.SetBool (IS_RUNNING,true);
                    break;
                }
            case PlayerStatus.ATTACKING:
                {
                    playerAnimator.SetBool(IS_ATTACKING,true);
                    ResetToIDLEState();
                    break;
                }
            case PlayerStatus.PICKUP:
                {
                    playerAnimator.SetBool(IS_PICKUP, true);
                    ResetToIDLEState();
                    break;
                }
            case PlayerStatus.HITTING:
                {
                    playerAnimator.SetBool(IS_HITTING, true);
                    ResetToIDLEState();
                    break;
                }
            case PlayerStatus.DEAD:
                {
                    playerAnimator.SetBool(IS_DEAD, true);
                    break ;
                }
        }
    }

    private void ResetToIDLEState()
    {
        AnimatorClipInfo[] animatorClips = playerAnimator.GetCurrentAnimatorClipInfo(0);
        const string IDLENAME = "KayKit Animated Character|Idle";


        if (animatorClips[0].clip.name == IDLENAME && playerStatus != PlayerStatus.IDLE)
        {
            playerStatus = PlayerStatus.IDLE;
        }

    }

    public Tile GetPlayerPosition()
    {
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), -transform.up);
        RaycastHit hitData;
        Debug.DrawRay(ray.origin, ray.direction * 2);

        if(Physics.Raycast(ray, out hitData, 2))
        {
            Tile tile = hitData.collider.gameObject.GetComponent<Tile>();
            tile.ChangeColor(0);
            return tile;
        }

        return null;
    }

    public void AnalizeMovablePath()
    {
        Tile playerTile = GetPlayerPosition();

        int xPos = (int)playerTile.positionX - 1;
        int yPos = (int)playerTile.positionY - 1;

        if (playerTile.tileType == Tile.TileType.BLACK)
        {
            //Top Path
            for(int i = xPos + 1; i < 21; i++)
            {
                Tile tile = gridManager.grid[i][yPos];
                if (tile.tileType == Tile.TileType.BLACK)
                {
                    tile.ChangeColor(1);
                    tile.tileStatus = Tile.TileStatus.MOVABLE;
                }
                else
                {
                    break;
                }
            }

            //Bottom Path

            for (int i = xPos - 1; i > -1; i--)
            {
                Tile tile = gridManager.grid[i][yPos];
                if (tile.tileType == Tile.TileType.BLACK)
                {
                    tile.ChangeColor(1);
                    tile.tileStatus = Tile.TileStatus.MOVABLE;
                }
                else
                {
                    break;
                }
            }

            //Left Path

            for (int i = yPos - 1; i > -1; i--)
            {
                Tile tile = gridManager.grid[xPos][i];
                if (tile.tileType == Tile.TileType.BLACK)
                {
                    tile.ChangeColor(1);
                    tile.tileStatus = Tile.TileStatus.MOVABLE;
                }
                else
                {
                    break;
                }
            }

            //Right Path
            for (int i = yPos + 1; i < 21; i++)
            {
                Tile tile = gridManager.grid[xPos][i];
                if (tile.tileType == Tile.TileType.BLACK)
                {
                    tile.ChangeColor(1);
                    tile.tileStatus = Tile.TileStatus.MOVABLE;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void DoAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit raycastHit;

            if(Physics.Raycast(ray, out raycastHit, Mathf.Infinity, layerMask))
            {
                Tile tile = raycastHit.collider.gameObject.GetComponent<Tile>();
                Debug.Log(tile.transform.position.x + " " + tile.transform.position.z);
            }
        }
    }
}
