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

    public int playerID;

    Animator playerAnimator;
    GridManager gridManager;

    [SerializeField]
    LayerMask layerMask;

    [SerializeField]
    private float moveSpeed = 10f, rotationSpeed = 10f;

    [SerializeField]
    int blackDownMoveLength = 3;

    private Vector3 movePosition;

    private GameManager gameManager;

    private void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        gameManager = FindAnyObjectByType<GameManager>();

        playerStatus = PlayerStatus.IDLE;
        playerAnimator = GetComponent<Animator>();
        ChangeAnimation();
    }

    private void Update()
    {
        ChangeAnimation();

        if(playerID == gameManager.currentPlayer && gameManager.isPlayerTurn)
        {
            if (playerStatus == PlayerStatus.IDLE)
            {
                AnalizeMovablePath();

                DoAction();
            }

            MovePlayer();
        }
        else
        {
            //ResetMovablePath();
        }
        
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

        /*int xPos = (int)playerTile.positionX - 1;
        int yPos = (int)playerTile.positionY - 1;*/

        int xPos = 0, yPos = 0;
        if (playerTile != null)
        {
            xPos = (int)playerTile.positionX - 1;
            yPos = (int)playerTile.positionY - 1;
        }

        if (gameManager.waveStatus == GameManager.GridWave.BLACKUP)
        {
            if(playerTile.tileType == Tile.TileType.BLACK)
            {
                for (int i = xPos + 1; i < 21; i++)
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
            
            else if (playerTile.tileType == Tile.TileType.WHITE)
            {
                for (int i = xPos + 1; i < 21; i++)
                {
                    Tile tile = gridManager.grid[i][yPos];
                    if (tile.tileType == Tile.TileType.WHITE)
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
                    if (tile.tileType == Tile.TileType.WHITE)
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
                    if (tile.tileType == Tile.TileType.WHITE)
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
                    if (tile.tileType == Tile.TileType.WHITE)
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

        else if (gameManager.waveStatus == GameManager.GridWave.BLACKDOWN)
        {
            int moveCount = 0;
            //Top Path
            for (int i = xPos + 1; i < 21; i++)
            {
                Tile tile = gridManager.grid[i][yPos];
                tile.ChangeColor(1);
                tile.tileStatus = Tile.TileStatus.MOVABLE;
                moveCount++;
                if(moveCount >= blackDownMoveLength)
                {
                    break;
                }
            }

            moveCount = 0;
            //Bottom Path

            for (int i = xPos - 1; i > -1; i--)
            {
                Tile tile = gridManager.grid[i][yPos];
                tile.ChangeColor(1);
                tile.tileStatus = Tile.TileStatus.MOVABLE;

                moveCount++;
                if (moveCount >= blackDownMoveLength)
                {
                    break;
                }
            }

            //Left Path
            moveCount = 0;
            for (int i = yPos - 1; i > -1; i--)
            {
                Tile tile = gridManager.grid[xPos][i];
                tile.ChangeColor(1);
                tile.tileStatus = Tile.TileStatus.MOVABLE;

                moveCount++;
                if (moveCount >= blackDownMoveLength)
                {
                    break;
                }
            }

            //Right Path
            moveCount = 0;
            for (int i = yPos + 1; i < 21; i++)
            {
                Tile tile = gridManager.grid[xPos][i];
                tile.ChangeColor(1);
                tile.tileStatus = Tile.TileStatus.MOVABLE;

                moveCount++;
                if (moveCount >= blackDownMoveLength)
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
                switch (tile.tileStatus)
                {
                    case Tile.TileStatus.MOVABLE:
                        {
                            movePosition = new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z);
                            if(Vector3.Distance(transform.position, mousePosition) >= 2)
                            {
                                playerStatus = PlayerStatus.RUNNING;
                            }
                            else
                            {
                                playerStatus = PlayerStatus.WALKING;
                            }

                            break;
                        }
                }

                ResetMovablePath();
            }

        }
    }

    private void MovePlayer()
    {
        if(playerStatus == PlayerStatus.WALKING || playerStatus == PlayerStatus.RUNNING)
        {
            if(transform.position != movePosition)
            {
                Vector3 direction = movePosition - transform.position;

                if(direction.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                transform.position = Vector3.MoveTowards(transform.position, movePosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                playerStatus = PlayerStatus.IDLE;
                gameManager.EndTurn();
            }
            
        }
    }

    public void ResetMovablePath()
    {
        /*Tile playerTile = GetPlayerPosition();
        int xPos = 0, yPos = 0;
        
        xPos = (int)playerTile.positionX - 1;
        yPos = (int)playerTile.positionY - 1;
       
        

        if (gameManager.waveStatus == GameManager.GridWave.BLACKUP)
        {
            //Top Path
            for (int i = xPos; i < 21; i++)
            {
                Tile tile = gridManager.grid[i][yPos];
                tile.ResetColor();
                tile.tileStatus = Tile.TileStatus.IDLE;
            }

            //Bottom Path

            for (int i = xPos; i > -1; i--)
            {
                Tile tile = gridManager.grid[i][yPos];
                tile.ResetColor();
                tile.tileStatus = Tile.TileStatus.IDLE;
            }

            //Left Path

            for (int i = yPos; i > -1; i--)
            {
                Tile tile = gridManager.grid[xPos][i];
                tile.ResetColor();
                tile.tileStatus = Tile.TileStatus.IDLE;
            }

            //Right Path
            for (int i = yPos; i < 21; i++)
            {
                Tile tile = gridManager.grid[xPos][i];
                tile.ResetColor();
                tile.tileStatus = Tile.TileStatus.IDLE;
            }
        }

        else if (gameManager.waveStatus == GameManager.GridWave.BLACKDOWN)
        {
            //Top Path
            for (int i = xPos + 1; i < 21; i++)
            {
                Tile tile = gridManager.grid[i][yPos];
                tile.ResetColor();
                tile.tileStatus = Tile.TileStatus.IDLE;
            }

            //Bottom Path

            for (int i = xPos - 1; i > -1; i--)
            {
                Tile tile = gridManager.grid[i][yPos];
                tile.ResetColor();
                tile.tileStatus = Tile.TileStatus.IDLE;
            }

            //Left Path

            for (int i = yPos - 1; i > -1; i--)
            {
                Tile tile = gridManager.grid[xPos][i];
                tile.ResetColor();
                tile.tileStatus = Tile.TileStatus.IDLE;
            }

            //Right Path
            for (int i = yPos + 1; i < 21; i++)
            {
                Tile tile = gridManager.grid[xPos][i];
                tile.ResetColor();
                tile.tileStatus = Tile.TileStatus.IDLE;
            }
        }*/

        for(int i = 0; i < 21; i++)
        {
            for (int j = 0; j < 21; j++)
            {
                Tile tile = gridManager.grid[i][j];
                tile.ResetColor();
                tile.tileStatus = Tile.TileStatus.IDLE;
            }
        }
    }
}
