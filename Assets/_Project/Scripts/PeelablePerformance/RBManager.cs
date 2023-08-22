using Stolenpad.Utils;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class RBManager : SingletonMB<RBManager>
{
    [SerializeField] private List<RBHandler> handlers = new List<RBHandler>();


    [SerializeField] private float Raduis;
    private JobHandle jobHandle;
    private AgentJob agentJob;
    private NativeArray<float3> rbPositions;    
    private NativeArray<bool> agentStates;

    public void AddAgent(RBHandler agent)
    {
        if (!handlers.Contains(agent))
        {
            handlers.Add(agent);
        }
    }
    public void RemoveAgent(RBHandler agent)
    {
        if (handlers.Contains(agent))
        {
            handlers.Remove(agent);
        }
    }

    private void Start()
    {
        JobUpdater();
    }
    //private void Update()
    //{
    //    JobUpdater();
    //}
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Raduis);
    }
    private void InitData()
    {
        rbPositions = new NativeArray<float3>(handlers.Count, Allocator.TempJob);
        agentStates = new NativeArray<bool>(handlers.Count, Allocator.TempJob);

        for (int i = 0; i < handlers.Count; i++)
        {
            rbPositions[i] = handlers[i].transform.position;
            agentStates[i] = false;
        }

        agentJob = new AgentJob
        {
            rbPositions = rbPositions,
            agentStates = agentStates,
            PlayerPosition = transform.position,
            Raduis= Raduis
        };
        jobHandle = agentJob.Schedule(handlers.Count, 100);

        jobHandle.Complete();

        GetDataFromJobs();
    }
    public void JobUpdater()
    {
        InitData();
    }
    private void GetDataFromJobs()
    {
        for (int i = 0; i < agentJob.agentStates.Length; i++)
        {
            handlers[i].CheckSwitch(agentJob.agentStates[i]);
        }
        rbPositions.Dispose();
        agentStates.Dispose();
    }
    [BurstCompile]
    public struct AgentJob : IJobParallelFor
    {
        public NativeArray<float3> rbPositions;
        public NativeArray<bool> agentStates;
        public float3 PlayerPosition;

        public float Raduis;
        public void Execute(int index)
        {
            bool InsideScreen = math.distance(rbPositions[index], PlayerPosition) > Raduis ? false : true;

            agentStates[index] = InsideScreen;
        }
    }
}
public struct RectBounds
{
    public float3 topRight;
    public float3 bottomLeft;
}
