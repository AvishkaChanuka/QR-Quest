using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    public bool isAlive = true;

    Animator playerAnimator;
    GridManager gridManager;

    [SerializeField]
    LayerMask layerMask;

    [SerializeField]
    private float moveSpeed = 10f, rotationSpeed = 10f;

    [SerializeField]
    int blackDownMoveLength = 3;

    [SerializeField]
    public int healthLevl = 10, maxHealth = 10, fightLevel = 10, maxEnergyLevel = 10, coinLevel = 0, attackStrength = 2;

    [SerializeField]
    private TextMeshProUGUI playerCoinAmountUI;

    [SerializeField]
    private Slider playerHealthUI, playerEnergyUI;

    [SerializeField]
    private Message Message;

    private Vector3 movePosition;

    private GameManager gameManager;
    private ChestChecker chestChecker;

    private bool doesHaveToCollect = false, doesHaveToAttack = false, doesHaveToUnlock = false;

    private Player enemy;
    private Chest chest;

    private void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        chestChecker = FindAnyObjectByType<ChestChecker>();

        playerStatus = PlayerStatus.IDLE;
        playerAnimator = GetComponent<Animator>();
        ChangeAnimation();

        playerHealthUI.maxValue = maxHealth;
        playerHealthUI.maxValue = maxEnergyLevel;
        UpdateUI();
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
        
        if(healthLevl <= 0)
        {
            isAlive = false;
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
                        if (tile.ISCollectableTile())
                        {
                            tile.ChangeColor(2);
                            tile.tileStatus = Tile.TileStatus.COLLECTABLE;
                            break;
                        }
                        else if (tile.IsChestTile() != null)
                        {
                            chest = tile.IsChestTile();
                            tile.ChangeColor(4);
                            tile.tileStatus = Tile.TileStatus.CHEST;
                            break;
                        }
                        else if(tile.IsPlayerAttackableTile() != null)
                        {
                            break;
                        }
                        else
                        {
                            tile.ChangeColor(1);
                            tile.tileStatus = Tile.TileStatus.MOVABLE;
                        }
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
                        if (tile.ISCollectableTile())
                        {
                            tile.ChangeColor(2);
                            tile.tileStatus = Tile.TileStatus.COLLECTABLE;
                            break;
                        }
                        else if (tile.IsChestTile() != null)
                        {
                            chest = tile.IsChestTile();
                            tile.ChangeColor(4);
                            tile.tileStatus = Tile.TileStatus.CHEST;
                            break;
                        }
                        else if (tile.IsPlayerAttackableTile() != null)
                        {
                            break;
                        }

                        else
                        {
                            tile.ChangeColor(1);
                            tile.tileStatus = Tile.TileStatus.MOVABLE;
                        }
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
                        if (tile.ISCollectableTile())
                        {
                            tile.ChangeColor(2);
                            tile.tileStatus = Tile.TileStatus.COLLECTABLE;
                            break;
                        }
                        else if (tile.IsChestTile() != null)
                        {
                            chest = tile.IsChestTile();
                            tile.ChangeColor(4);
                            tile.tileStatus = Tile.TileStatus.CHEST;
                            break;
                        }
                        else if (tile.IsPlayerAttackableTile() != null)
                        {
                            break;
                        }

                        else
                        {
                            tile.ChangeColor(1);
                            tile.tileStatus = Tile.TileStatus.MOVABLE;
                        }
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
                        if (tile.ISCollectableTile())
                        {
                            tile.ChangeColor(2);
                            tile.tileStatus = Tile.TileStatus.COLLECTABLE;
                            break;
                        }
                        else if (tile.IsChestTile() != null)
                        {
                            chest = tile.IsChestTile();
                            tile.ChangeColor(4);
                            tile.tileStatus = Tile.TileStatus.CHEST;
                            break;
                        }
                        else if (tile.IsPlayerAttackableTile() != null)
                        {
                            break;
                        }

                        else
                        {
                            tile.ChangeColor(1);
                            tile.tileStatus = Tile.TileStatus.MOVABLE;
                        }
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
                    else if (tile.IsPlayerAttackableTile() != null)
                    {
                        break;
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
                    else if (tile.IsPlayerAttackableTile() != null)
                    {
                        break;
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
                    else if (tile.IsPlayerAttackableTile() != null)
                    {
                        break;
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
                    else if (tile.IsPlayerAttackableTile() != null)
                    {
                        break;
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

                if (tile.IsPlayerAttackableTile() != null)
                {
                    enemy = tile.IsPlayerAttackableTile();
                    tile.ChangeColor(3);
                    tile.tileStatus = Tile.TileStatus.ATTACK;
                    break;
                }
                else if(tile.IsChestTile() != null)
                {
                    chest = tile.IsChestTile();
                    tile.ChangeColor(4);
                    tile.tileStatus = Tile.TileStatus.CHEST;
                    break;
                }
                else
                {
                    tile.ChangeColor(1);
                    tile.tileStatus = Tile.TileStatus.MOVABLE;
                    moveCount++;
                }
                
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

                if (tile.IsPlayerAttackableTile() != null)
                {
                    enemy = tile.IsPlayerAttackableTile();
                    tile.ChangeColor(3);
                    tile.tileStatus = Tile.TileStatus.ATTACK;
                    break;
                }
                else if (tile.IsChestTile() != null)
                {
                    chest = tile.IsChestTile();
                    tile.ChangeColor(4);
                    tile.tileStatus = Tile.TileStatus.CHEST;
                    break;
                }
                else
                {
                    tile.ChangeColor(1);
                    tile.tileStatus = Tile.TileStatus.MOVABLE;
                    moveCount++;
                }

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

                if (tile.IsPlayerAttackableTile() != null)
                {
                    enemy = tile.IsPlayerAttackableTile();
                    tile.ChangeColor(3);
                    tile.tileStatus = Tile.TileStatus.ATTACK;
                    break;
                }
                else if (tile.IsChestTile() != null)
                {
                    chest = tile.IsChestTile();
                    tile.ChangeColor(4);
                    tile.tileStatus = Tile.TileStatus.CHEST;
                    break;
                }
                else
                {
                    tile.ChangeColor(1);
                    tile.tileStatus = Tile.TileStatus.MOVABLE;
                    moveCount++;
                }

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

                if (tile.IsPlayerAttackableTile() != null)
                {
                    enemy = tile.IsPlayerAttackableTile();
                    tile.ChangeColor(3);
                    tile.tileStatus = Tile.TileStatus.ATTACK;
                    break;
                }
                else if (tile.IsChestTile() != null)
                {
                    chest = tile.IsChestTile();
                    tile.ChangeColor(4);
                    tile.tileStatus = Tile.TileStatus.CHEST;
                    break;
                }
                else
                {
                    tile.ChangeColor(1);
                    tile.tileStatus = Tile.TileStatus.MOVABLE;
                    moveCount++;
                }

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
                            playerStatus = PlayerStatus.RUNNING;
                            break;
                        }
                    case Tile.TileStatus.COLLECTABLE:
                        {
                            movePosition = new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z);
                            playerStatus = PlayerStatus.RUNNING;
                            doesHaveToCollect = true;
                            break;
                        }
                    case Tile.TileStatus.ATTACK:
                        {
                            movePosition = new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z);
                            Vector3 dirVector = movePosition - transform.position;
                            dirVector.Normalize();
                            movePosition -= dirVector * 2;

                            Debug.Log(dirVector * 2);

                            playerStatus = PlayerStatus.RUNNING;
                            doesHaveToAttack = true;
                            break;
                        }
                    case Tile.TileStatus.CHEST:
                        {
                            movePosition = new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z);
                            Vector3 dirVector = movePosition - transform.position;
                            dirVector.Normalize();
                            movePosition -= dirVector * 2;

                            Debug.Log(dirVector * 2);

                            playerStatus = PlayerStatus.RUNNING;
                            doesHaveToUnlock = true;
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
            Vector3 direction = movePosition - transform.position;

            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            

            if (transform.position != movePosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, movePosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                playerStatus = PlayerStatus.IDLE;

                if (doesHaveToCollect)
                {
                    playerStatus = PlayerStatus.PICKUP;
                    Debug.Log("Collected");
                    doesHaveToCollect = false;
                }
                else if (doesHaveToAttack)
                {
                    playerStatus = PlayerStatus.ATTACKING;
                    enemy.GetAttack(2);
                    Debug.Log("Attack");
                    doesHaveToAttack = false;
                }
                else if (doesHaveToUnlock)
                {
                    if (gameManager.waveStatus == GameManager.GridWave.BLACKUP)
                    {
                        if (chestChecker.IsBothPlayersinArea)
                        {
                            AttackToChest();
                        }
                        else
                        {
                            Message.ShowMessage("To attack to the chest, Both players should be in square while black tiles are Up!");
                        }
                    }
                    else
                    {
                        AttackToChest();
                    }

                    doesHaveToUnlock = false;
                }
                
                gameManager.EndTurn();
            }
            
        }
    }

    void AttackToChest()
    {
        if(fightLevel >= attackStrength)
        {
            playerStatus = PlayerStatus.ATTACKING;
            chest.GetAttack(attackStrength);
            EnergyizePlayer(-2);

            if (chest.IsChestClaimed())
            {
                EarnCoin(chest.chestValue);
                Message.ShowMessage(gameObject.name + " has claimed a chest!!!");
                gameManager.noOfChests--;
                chest.gameObject.SetActive(false);
            }
        }
        else
        {
            Message.ShowMessage("Lack of Energy!, Collect Energizers and try again.");
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectable"))
        {
            CollectableObject item = other.gameObject.GetComponent<CollectableObject>();
            switch (item.itemType)
            {
                case CollectableObject.ItemType.HEAL:
                    {
                        HealPlayer(item.ItemValue);
                        item.Destroy();
                        break;
                    }
                case CollectableObject.ItemType.ENERGY:
                    {
                        EnergyizePlayer(item.ItemValue);
                        item.Destroy();
                        break;
                    }
                case CollectableObject.ItemType.COIN:
                    {
                        EarnCoin(item.ItemValue);
                        item.Destroy();
                        break;
                    }
            }
        }
    }

    public void ResetMovablePath()
    {

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

    public void HealPlayer(int value)
    {
        healthLevl = healthLevl + value;
        healthLevl = Mathf.RoundToInt(Mathf.Clamp((float)healthLevl, 0f, (float)maxHealth));
        UpdateUI();
        Debug.Log(healthLevl);
    }

    public void EnergyizePlayer(int value)
    {
        fightLevel = fightLevel + value;
        fightLevel = Mathf.RoundToInt(Mathf.Clamp((float)fightLevel, 0f, (float)maxEnergyLevel));
        UpdateUI();
    }

    public void GetAttack(int value)
    {
        healthLevl -= value;
        healthLevl = Mathf.RoundToInt(Mathf.Clamp((float)healthLevl, 0f, (float)maxHealth));
        UpdateUI();

        if(healthLevl <= 0)
        {
            isAlive = false;
        }
    }

    public void DoAttack()
    {

    }

    public void EarnCoin(int value)
    {
        coinLevel += value;
        UpdateUI();
    }

    void UpdateUI()
    {
        playerCoinAmountUI.text = coinLevel.ToString();
        playerHealthUI.value = healthLevl;
        playerEnergyUI.value = fightLevel;
    }
}
