using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UltimateJoystickExample.Spaceship;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public enum AIState
{
    Idle,
    Scrapping,
    Building,
    Painting
}
public enum AIInternalState
{
    Waiting,
    MovingToPosition,
    Working
}

public enum AITilePosition
{
    Min,
    Max
}
public class AIManager : MonoBehaviour
{
    public AIState state;
    public AIInternalState aIInternalState;
    public PeelableManager peelableManager;
    [SerializeField] private Transform FrowardHitPoint;
    [SerializeField] private LayerMask LayerMask;
    [SerializeField] private Rigidbody rb;
    [SerializeField] float3? Target;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float RotationSpeed;
    [SerializeField] RBSplitter Splitter;
    private int row;
    [SerializeField] private bool IsMinStart;
    [SerializeField] private bool isEnd;
    [SerializeField] private bool isChangingRow;
    [SerializeField] private Animator anim;
    [SerializeField] private bool drive;
    private bool move;
    public bool canRotate;
    [SerializeField] private float speedMultiplayer = 100;
    [SerializeField] private GameObject scrapeToolHolder;
    public ScrapeTool scrapeTool;
    [SerializeField] private Collector Collector;
    private void Start()    
    {
    }
    private void FixedUpdate()
    {
        TakeInternalDecision();
    }
    private void TakeInternalDecision()
    {
        switch (aIInternalState)
        {
            case AIInternalState.Waiting:

                break;
            case AIInternalState.MovingToPosition:
                if (Target == null)
                {
                    MoveToStartPosition();
                }

                MoveToPosition();
                break;
            case AIInternalState.Working:

                break;
        }
    }
    private void MoveToStartPosition()
    {
        var t = Splitter.Grid.GetCell(row, 0);
        Target = t.MinPosition;
        //row++;
    }
    private void MoveToNextPosition()
    {
        if (isEnd)
        {
            row++;
            isChangingRow = true;
            IsMinStart = !IsMinStart;

        }

        isEnd = !isEnd;

        if (isEnd)
        {
            if (IsMinStart)
            {
                var t = Splitter.Grid.GetCell(row, 0);
                if (t == null)
                {
                    aIInternalState = AIInternalState.Waiting;

                    return;
                }
                Target = t.MaxPosition;
            }
            else
            {
                var t = Splitter.Grid.GetCell(row, 0);
                if (t == null)
                {
                    aIInternalState = AIInternalState.Waiting;

                    return;
                }
                Target = t.MinPosition;
            }
        }
        else
        {
            if (IsMinStart)
            {
                var t = Splitter.Grid.GetCell(row, 0);
                if (t == null)
                {
                    aIInternalState = AIInternalState.Waiting;

                    return;
                }
                Target = t.MinPosition;
            }
            else
            {
                var t = Splitter.Grid.GetCell(row, 0);
                if (t == null)
                {
                    aIInternalState = AIInternalState.Waiting;
                    return;

                }
                Target = t.MaxPosition;
            }
        }

    }

    private void MoveToPosition()
    {
        rb.velocity = Vector3.zero;

        float3 position = transform.position;

        var direction = (float3)Target - (float3)transform.position;
        direction.y = 0f;

        var horizontalDirection = float3.zero;
        horizontalDirection.x = direction.x;
        horizontalDirection.z = direction.z;

        var rotation = quaternion.LookRotationSafe(horizontalDirection, math.up());

        position += math.normalize(direction) * MoveSpeed * Time.deltaTime;

        rb.transform.SetPositionAndRotation(position, rotation);

        if (math.distance(transform.position, (float3)Target) <= 0.2f)
        {
            MoveToNextPosition();
            if (isChangingRow)
            {
                isChangingRow = false;
            }
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
                if (scrapeToolHolder.gameObject.activeInHierarchy)
                {
                    if (scrapeToolHolder.gameObject.activeInHierarchy)
                    {
                        if (scrapeTool.showing)
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
                    if (scrapeToolHolder.gameObject.activeInHierarchy)
                    {
                        if (scrapeTool.toolAngleController != null)
                            anim.SetBool("Push", false);
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
}