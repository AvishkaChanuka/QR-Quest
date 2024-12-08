using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        playerStatus = PlayerStatus.IDLE;
        playerAnimator = GetComponent<Animator>();
        ChangeAnimation();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            playerStatus = PlayerStatus.ATTACKING;
            Debug.Log("Attack");
        }

        ChangeAnimation();
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
}
