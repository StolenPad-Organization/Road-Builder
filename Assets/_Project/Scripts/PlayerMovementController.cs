using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public UltimateJoystick joystick;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float paintMoveSpeed = 5f;
    [SerializeField] private float angleSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float originalRotationSpeed = 5.0f;
    [SerializeField] private Animator anim;
    private Vector3 moveDirection = Vector3.zero;
    public bool canMove = false;
    public bool canRotate;
    [SerializeField] private float speedMultiplayer = 100;
    private PlayerController playerController;
    [SerializeField] private bool drive;
    private bool move;
    [SerializeField] private Rigidbody rb;
    private RBManagerJobs rbManager;
    private GameManager gameManager;
    public bool insideAngleTrigger;

    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        playerController = PlayerController.instance;
        rbManager = RBManagerJobs.Instance;
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
            speedMultiplayer += 60 * Time.deltaTime;
            if (speedMultiplayer > 100)
                speedMultiplayer = 100;
        }

        horizontalInput = joystick.HorizontalAxis;
        verticalInput = joystick.VerticalAxis;
        moveDirection.x = horizontalInput;
        moveDirection.z = verticalInput;
        moveDirection.Normalize();
        //SetWalkSpeed(moveDirection.magnitude);
        SetPushSpeed();
        if (drive && !anim.GetBool("Drive") || !drive && anim.GetBool("Drive"))
        {
            anim.SetBool("Drive", drive);
        }

       // if (moveDirection.magnitude <= 0.3f) return;

        if (insideAngleTrigger)
        {
            moveDirection *= (angleSpeed * (speedMultiplayer / 100));
        }
        else
        {
            if (playerController.paintMachine == null && playerController.asphaltMachine == null)
                moveDirection *= (moveSpeed * (speedMultiplayer / 100));
            else
            {
                if (playerController.paintMachine != null)
                    moveDirection *= (paintMoveSpeed * (speedMultiplayer / 100));
                if (playerController.asphaltMachine != null)
                    moveDirection *= (playerController.asphaltMachine.Speed * (speedMultiplayer / 100));
            }
        }

        if (moveDirection != Vector3.zero)
        {
            //transform.rotation = Quaternion.LookRotation(moveDirection);
            if (canRotate)
                Rotate();
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
        //if (gameManager.currentZone != null)
        //{
        //    if (gameManager.currentZone.zoneState == ZoneState.PeelingStage)
        //        rbManager.UpdateJobs();
        //}
    }

    private void Rotate()
    {
        //rb.MoveRotation(Quaternion.LookRotation(moveDirection));

        float targetAngleRadians = Mathf.Atan2(horizontalInput, verticalInput);
        float targetAngleDegrees = targetAngleRadians * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngleDegrees, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void SetRotationSpeed()
    {
        rotationSpeed = originalRotationSpeed;
    }
    public void SetRotationSpeed(float _rotationSpeed)
    {
        rotationSpeed = _rotationSpeed;
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

    public void SetAnimation()
    {
        if (anim == null) return;
        anim.SetBool("Drive", drive);
        if (!drive)
        {
            if (!move)
            {
                anim.SetBool("Push", false);
                anim.SetBool("Walk", false);
                anim.SetBool("Strafe", false);
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

                    if(playerController.paintMachine != null)
                    {
                        if(playerController.paintMachine.toolAngleController != null)
                        {
                            anim.SetBool("Push", !move);
                            anim.SetBool("Walk", move);
                        }
                    }

                    if (playerController.scrapeToolHolder.gameObject.activeInHierarchy)
                    {
                        if (playerController.scrapeTool.toolAngleController != null)
                            anim.SetBool("Push", false);
                    }

                    if (!canRotate)
                    {
                        anim.SetBool("Strafe", true);
                    }
                    else
                    {
                        anim.SetBool("Strafe", false);
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
            anim.SetBool("Strafe", false);
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
