using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;


public enum ScrappingAIInternalState
{
    Waiting,
    MovingToJob,
    Working,
    MovingToSell,
    WaitForSell,
    BackFromSell,
    WaitingForSellPoint
}

public class ScrappingAIController : PlayerMovementController
{
    [Header("AI Movement")] 
    public ScrappingAIInternalState aIInternalState;
    [SerializeField] float3? Target;
    [SerializeField] RBSplitter Splitter;
    private int row;
    [SerializeField] private bool IsMinStart;
    [SerializeField] private bool isEnd;
    [SerializeField] private bool isChangingRow;
    [SerializeField] private bool isFull;
    [SerializeField] private float3 LastPosition;   
    [SerializeField] private float3 LastTargetPosition;
    [SerializeField] private AISellPoint AISellPoint;
    [SerializeField] private float sellRate;    
    private float currentSellRate;
    private int sellCount;
    [SerializeField] private float nextSell;
    private void OnEnable()
    {
        playerController.OnFullCollectables += OnFullCollectables;
        AISellPoint.OnSellPointAvailble += OnSellPointAvailble;
    }
    private void OnDisable()
    {
        playerController.OnFullCollectables -= OnFullCollectables;
        AISellPoint.OnSellPointAvailble -= OnSellPointAvailble;
    }

    protected override void FixedUpdate()
    {

        if (speedMultiplayer != speedMultiplayerMax && canRecoverSpeed)
        {
            speedMultiplayer += 60 * Time.deltaTime;
            if (speedMultiplayer > speedMultiplayerMax)
                speedMultiplayer = speedMultiplayerMax;
        }

        TakeInternalDecision();
       
        if (!canMove) return;

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
    }

    private void TakeInternalDecision()
    {
        switch (aIInternalState)
        {
            case ScrappingAIInternalState.Waiting:

                break;
            case ScrappingAIInternalState.MovingToJob:
                if (Target == null)
                {
                    MoveToStartPosition();
                }

                MoveToPosition();
                break;
            case ScrappingAIInternalState.Working:

                break;
            case ScrappingAIInternalState.MovingToSell:
                MoveToSellPosition();

                break;
            case ScrappingAIInternalState.WaitForSell:

                if (nextSell <= 0)
                {
                    playerController.AISellCollectable(AISellPoint, this);
                    sellCount++;
                    nextSell = currentSellRate - (sellCount * 0.005f);
                    if (nextSell < 0.01f) nextSell = 0.01f;
                    if (playerController.EmptyCollectables)
                    {
                        canMove = true;
                        aIInternalState = ScrappingAIInternalState.BackFromSell;
                    }
                }
                else
                {
                    nextSell -= Time.deltaTime;
                }
                break;
            case ScrappingAIInternalState.BackFromSell:
                BackFromSellPosition();
                break;
            case ScrappingAIInternalState.WaitingForSellPoint:

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
                    aIInternalState = ScrappingAIInternalState.Waiting;

                    return;
                }
                Target = t.MaxPosition;
            }
            else
            {
                var t = Splitter.Grid.GetCell(row, 0);
                if (t == null)
                {
                    aIInternalState = ScrappingAIInternalState.Waiting;

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
                    aIInternalState = ScrappingAIInternalState.Waiting;

                    return;
                }
                Target = t.MinPosition;
            }
            else
            {
                var t = Splitter.Grid.GetCell(row, 0);
                if (t == null)
                {
                    aIInternalState = ScrappingAIInternalState.Waiting;
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

        var direction = (float3)Target - position;
        direction.y = 0f;

        var horizontalDirection = float3.zero;
        horizontalDirection.x = direction.x;
        horizontalDirection.z = direction.z;

        horizontalInput = horizontalDirection.x;
        verticalInput = horizontalDirection.z;


        //var rotation = quaternion.LookRotationSafe(horizontalDirection, math.up());

        //position += math.normalize(direction) * MoveSpeed * Time.deltaTime;

        //rb.transform.SetPositionAndRotation(position, rotation);

        if (math.distance(position, (float3)Target) <= 0.2f)
        {
            MoveToNextPosition();
            if (isChangingRow)
            {
                isChangingRow = false;
            }
        }
    }

    private void MoveToSellPosition()
    {   
        rb.velocity = Vector3.zero;

        float3 position = transform.position;

        var direction = (float3)AISellPoint.transform.position - position;
        direction.y = 0f;

        var horizontalDirection = float3.zero;
        horizontalDirection.x = direction.x;
        horizontalDirection.z = direction.z;

        horizontalInput = horizontalDirection.x;
        verticalInput = horizontalDirection.z;

        if (math.distance(position, (float3)AISellPoint.transform.position) <= 0.2f)
        {
            aIInternalState = ScrappingAIInternalState.WaitForSell;
            canMove = false;
            // MoveToNextPosition();
        }
    }
    private void BackFromSellPosition() 
    {
        rb.velocity = Vector3.zero;

        float3 position = transform.position;

        var direction = LastPosition - position;
        direction.y = 0f;

        var horizontalDirection = float3.zero;
        horizontalDirection.x = direction.x;
        horizontalDirection.z = direction.z;

        horizontalInput = horizontalDirection.x;
        verticalInput = horizontalDirection.z;

        if (math.distance(position,LastPosition) <= 0.2f)
        {
            aIInternalState = ScrappingAIInternalState.MovingToJob;
            Target = LastTargetPosition;
            // MoveToNextPosition();
        }
    }
    private void OnFullCollectables()
    {

        aIInternalState = ScrappingAIInternalState.MovingToSell;
        LastPosition = transform.position;
        LastTargetPosition= (float3)Target;
        Target = AISellPoint.transform.position;
    }

    public void CheckIfCanWorkOnSell()
    {
        if(playerController.collectablesLimit <= playerController.collectables.Count)
        {
            aIInternalState = ScrappingAIInternalState.WaitingForSellPoint;
        }
        else
        {
            canMove = true;
            aIInternalState = ScrappingAIInternalState.BackFromSell;
        }
    }
    public void OnSellPointAvailble()
    {
        if(aIInternalState == ScrappingAIInternalState.WaitingForSellPoint)
        {
            aIInternalState = ScrappingAIInternalState.WaitForSell;
        }
    }
}
