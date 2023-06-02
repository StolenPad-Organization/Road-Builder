using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private UltimateJoystick joystick;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float paintMoveSpeed = 5f;
    [SerializeField] private Animator anim;
    private Vector3 moveDirection;
    public bool canMove = false;
    [SerializeField] private float speedMultiplayer = 100;
    private PlayerController playerController;
    private bool drive;
    private bool move;

    void Start()
    {
        playerController = PlayerController.instance;
    }

    public void SetSpeedMultiplayer(float amount)
    {
        speedMultiplayer = amount;
    }

    void Update()
    {
        
        if (!canMove) return;
        float horizontalInput = joystick.HorizontalAxis;
        float verticalInput = joystick.VerticalAxis;

        if(playerController.paintMachine == null)
            moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized * (moveSpeed * (speedMultiplayer / 100));
        else
            moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized * (paintMoveSpeed * (speedMultiplayer / 100));

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
            ToggleMoveAnimation(true);
        }
        else
        {
            ToggleMoveAnimation(false);
        }

        transform.Translate(moveDirection * Time.deltaTime, Space.World);

        if (speedMultiplayer != 100)
        {
            speedMultiplayer += 30 * Time.deltaTime;
            if (speedMultiplayer > 100)
                speedMultiplayer = 100;
        }
        SetPushSpeed();
    }

    public void ToggleMovementAnimation(bool activate)
    {
        move = activate;
        drive = !activate;
        SetAnimation();
    }

    private void ToggleMoveAnimation(bool activate)
    {
        if (drive || move == activate) return;
        move = activate;
        SetAnimation();
    }

    private void SetAnimation()
    {
        anim.SetBool("Drive", drive);
        if (!drive)
        {
            if (!move)
            {
                anim.SetBool("Push", false);
                anim.SetBool("Walk", false);
            }
            else
            {
                if (playerController.scrapeToolHolder.gameObject.activeInHierarchy || playerController.paintMachine != null || 
                    (playerController.asphaltMachine != null && !playerController.asphaltMachine.drivable))
                {
                    anim.SetBool("Push", move);
                    anim.SetBool("Walk", !move);
                }
                else
                {
                    anim.SetBool("Push", !move);
                    anim.SetBool("Walk", move);
                }
            }
        }
        else
        {
            anim.SetBool("Push", false);
            anim.SetBool("Walk", false);
        }
    }

    private void SetPushSpeed()
    {
        anim.SetFloat("PushSpeed", speedMultiplayer / 100);
    }
}
