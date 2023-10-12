using System;
using System.Collections;
using System.Collections.Generic;
using UltimateJoystickExample.Spaceship;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public enum BuildingAIInternalState
{
    Waiting,
    GoToInitReload,
    MovingToJob,
    Working,    
    MovingToReload,
    WaitForReload,
    BackFromReload
}
public class BuildingAIController : PlayerMovementController
{
    [Header("AI Movement")]
    public BuildingAIInternalState aIInternalState;
    [SerializeField] float3? Target;
    [SerializeField] RBSplitter Splitter;
    private int row;
    [SerializeField] private bool IsMinStart;
    [SerializeField] private bool isEnd;
    [SerializeField] private bool isChangingRow;
    [SerializeField] private bool isFull;
    [SerializeField] private float3? LastPosition;
    [SerializeField] private float3? LastTargetPosition;
        
    [SerializeField] private Transform ReloadPosition;
    [SerializeField] private float sellRate;
    [SerializeField] private BuildMachine BuildMachine;

    private float currentSellRate;  
    private int sellCount;
    [SerializeField] private float nextSell;

    private void Start()
    {
       
    }
    private void OnEnable()
    {
        BuildMachine.OnEmptyAmmo += OnEmptyAmmo;    
        BuildMachine.OnFullAmmo += OnFullAmmo;
    }

    
    private void OnDisable()
    {
        BuildMachine.OnFullAmmo -= OnFullAmmo;
        BuildMachine.OnEmptyAmmo -= OnEmptyAmmo;
    }

    private void OnEmptyAmmo()
    {
         aIInternalState = BuildingAIInternalState.MovingToReload;
        if(Target!= null)
        {
            LastPosition = transform.position;
            LastTargetPosition = (float3)Target;
            Target = ReloadPosition.position;
        }
    }

    private void OnFullAmmo()
    {
        if(Target != null)
        {
            aIInternalState = BuildingAIInternalState.BackFromReload;
        }
        else
        {
            aIInternalState = BuildingAIInternalState.MovingToJob;
        }
        canMove = true;
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
            case BuildingAIInternalState.GoToInitReload:
                
                break;
            case BuildingAIInternalState.MovingToJob:
                if (Target == null)
                {
                    MoveToStartPosition();
                    Debug.Log("pdjzodz");
                }

                MoveToPosition();
                break;
            case BuildingAIInternalState.Working:

                break;
            case BuildingAIInternalState.MovingToReload:
                MoveToReloadPosition();

                break;
            case BuildingAIInternalState.WaitForReload:

                break;
            case BuildingAIInternalState.BackFromReload:
                if(LastPosition == null)
                {
                    aIInternalState = BuildingAIInternalState.MovingToJob;
                    Target = null;
                    return;
                }
                BackFromReloadPosition();
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
                    aIInternalState = BuildingAIInternalState.Waiting;

                    return;
                }
                Target = t.MaxPosition;
            }
            else
            {
                var t = Splitter.Grid.GetCell(row, 0);
                if (t == null)
                {
                    aIInternalState = BuildingAIInternalState.Waiting;

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
                    aIInternalState = BuildingAIInternalState.Waiting;

                    return;
                }
                Target = t.MinPosition;
            }
            else
            {
                var t = Splitter.Grid.GetCell(row, 0);
                if (t == null)
                {
                    aIInternalState = BuildingAIInternalState.Waiting;

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

        var distance = math.distance(position, (float3)Target);

        if (distance <= 6f && !playerController.ReadyToBuild)
        {
            playerController.ReadyToBuild = true;
        }
        if (distance <= 0.3f)
        {
            MoveToNextPosition();
            if (isChangingRow)
            {
                isChangingRow = false;
            }
        }
    }

    private void MoveToReloadPosition()
    {
        rb.velocity = Vector3.zero;

        float3 position = transform.position;   

        var direction = (float3)ReloadPosition.position - position;
        direction.y = 0f;

        var horizontalDirection = float3.zero;
        horizontalDirection.x = direction.x;
        horizontalDirection.z = direction.z;

        horizontalInput = horizontalDirection.x;
        verticalInput = horizontalDirection.z;


       
        if (math.distance(position, (float3)ReloadPosition.position) <= 0.3f)
        {
            playerController.ReadyToBuild = false;

            aIInternalState = BuildingAIInternalState.WaitForReload;

            canMove = false;
            // MoveToNextPosition();
        }
    }
    private void BackFromReloadPosition()
    {
        if (LastPosition == null) return;

        rb.velocity = Vector3.zero;

        float3 position = transform.position;

        var direction = (float3)LastPosition - position;
        direction.y = 0f;

        var horizontalDirection = float3.zero;
        horizontalDirection.x = direction.x;
        horizontalDirection.z = direction.z;

        horizontalInput = horizontalDirection.x;
        verticalInput = horizontalDirection.z;

        var distance = math.distance(position, (float3)LastPosition);

        if (distance <= 6f && !playerController.ReadyToBuild)
        {
            playerController.ReadyToBuild = true;
        }

        if (distance <= 0.3f)
        {
            playerController.ReadyToBuild = true;
            aIInternalState = BuildingAIInternalState.MovingToJob;
            Target = LastTargetPosition;
            // MoveToNextPosition();
        }
    }
}
