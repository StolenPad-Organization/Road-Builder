using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public UltimateJoystick joystick;
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float paintMoveSpeed = 5f;
    [SerializeField] protected float angleSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float originalRotationSpeed = 5.0f;
    [SerializeField] protected Animator anim;
    protected Vector3 moveDirection = Vector3.zero;
    public bool canMove = false;
    public bool canRotate;
    [SerializeField] protected float speedMultiplayer = 100;
    [SerializeField] protected float speedMultiplayerMax = 100;
    public bool canRecoverSpeed;
    public PlayerController playerController;
    [SerializeField] protected bool drive;
    protected bool move;
    [SerializeField] protected Rigidbody rb;
    private RBManagerJobs rbManager;
    private GameManager gameManager;
    public bool insideAngleTrigger;

    protected float horizontalInput;
    protected float verticalInput;

    private float walkType;



    private void Start()
    {
        //playerController = GameManager.instance.player;
        rbManager = RBManagerJobs.Instance;
        gameManager = GameManager.instance;
    }

    public void SetSpeedMultiplayer(float amount)
    {
        speedMultiplayer = amount;
    }

   protected virtual void FixedUpdate()
    {
        if (!canMove) return;

        if (speedMultiplayer != speedMultiplayerMax && canRecoverSpeed)
        {
            speedMultiplayer += 60 * Time.deltaTime;
            if (speedMultiplayer > speedMultiplayerMax)
                speedMultiplayer = speedMultiplayerMax;
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

    protected void Rotate()
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

    protected void ToggleMoveAnimation(bool activate)
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
                //if(playerController == null)
                //    playerController = GameManager.instance.player;

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

    protected void SetPushSpeed()
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

    public void SetWalkType(float _walkType, int _maxspeed)
    {
        DOTween.To(() => walkType, x => walkType = x, _walkType, 0.5f)
            .OnUpdate(() => anim.SetFloat("WalkType", walkType))
            .OnComplete(() => 
            { 
                if(_walkType != 0)
                {
                    speedMultiplayerMax = _maxspeed;
                }
                else
                {
                    speedMultiplayerMax = 100;
                }
                speedMultiplayer = speedMultiplayerMax;
            });
    }

    
}
