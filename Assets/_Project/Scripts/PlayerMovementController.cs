using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public UltimateJoystick joystick;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float paintMoveSpeed = 5f;
    [SerializeField] private Animator anim;
    private Vector3 moveDirection = Vector3.zero;
    public bool canMove = false;
    public bool canRotate;
    [SerializeField] private float speedMultiplayer = 100;
    private PlayerController playerController;
    [SerializeField] private bool drive;
    private bool move;
    [SerializeField] private Rigidbody rb;
    private RBManager rbManager;
    private GameManager gameManager;

    void Start()
    {
        playerController = PlayerController.instance;
        rbManager = RBManager.Instance;
        gameManager = GameManager.instance;
    }

    public void SetSpeedMultiplayer(float amount)
    {
        speedMultiplayer = amount;
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        if (speedMultiplayer != 100)
        {
            speedMultiplayer += 30 * Time.deltaTime;
            if (speedMultiplayer > 100)
                speedMultiplayer = 100;
        }

        float horizontalInput = joystick.HorizontalAxis;
        float verticalInput = joystick.VerticalAxis;
        moveDirection.x = horizontalInput;
        moveDirection.z = verticalInput;
        moveDirection.Normalize();
        //SetWalkSpeed(moveDirection.magnitude);
        SetPushSpeed();
        if (drive && !anim.GetBool("Drive") || !drive && anim.GetBool("Drive"))
        {
            anim.SetBool("Drive", drive);
        }

        //if (moveDirection.magnitude <= 0.3f) return;

        if(playerController.paintMachine == null && playerController.asphaltMachine == null)
            moveDirection *= (moveSpeed * (speedMultiplayer / 100));
        else
        {
            if(playerController.paintMachine != null)
                moveDirection *= (paintMoveSpeed * (speedMultiplayer / 100));
            if (playerController.asphaltMachine != null)
                moveDirection *= (playerController.asphaltMachine.Speed * (speedMultiplayer / 100));
        }
            

        if (moveDirection != Vector3.zero)
        {
            //transform.rotation = Quaternion.LookRotation(moveDirection);
            if(canRotate)
                rb.MoveRotation(Quaternion.LookRotation(moveDirection));
            ToggleMoveAnimation(true);
        }
        else
        {
            ToggleMoveAnimation(false);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        //transform.Translate(moveDirection * Time.deltaTime, Space.World);
        rb.MovePosition(transform.position + moveDirection * Time.deltaTime);
        if(gameManager.currentZone != null)
        {
            if (gameManager.currentZone.zoneState == ZoneState.PeelingStage)
                rbManager.JobUpdater();
        }
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
                if(playerController == null)
                    playerController = PlayerController.instance;

                if (playerController.scrapeToolHolder.gameObject.activeInHierarchy || playerController.paintMachine != null || 
                    (playerController.asphaltMachine != null && !playerController.asphaltMachine.drivable))
                {
                    if (playerController.scrapeToolHolder.gameObject.activeInHierarchy)
                    {
                        if (playerController.scrapeTool.showing)
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
                    else
                    {
                        anim.SetBool("Push", move);
                        anim.SetBool("Walk", !move);
                    }
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

    private void SetWalkSpeed(float t)
    {
        anim.SetFloat("WalkSpeed", t);
    }

    public bool MovementCheck()
    {
        return moveDirection != Vector3.zero;
    }
}
