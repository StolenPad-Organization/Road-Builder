using Stolenpad.Utils;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class RBManagerJobs : SingletonMB<RBManagerJobs>
{
    [SerializeField] private RBTile[] rbTiles;
    [SerializeField] private float radius;
    [SerializeField] private Transform target;

    [SerializeField] private List<RBHandler> handlers = new List<RBHandler>();
    private Dictionary<byte, List<RBHandler>> subscribedNewHandlers = new Dictionary<byte, List<RBHandler>>();

    private JobHandle jobHandle;
    private AgentJob agentJob;
    private NativeArray<float3> rbPositions;
    private NativeArray<bool> agentStates;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        UpdateJobs();
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    protected void InitializeIt()
    {
        DisposeNativeArrays();
        InitializeHandlers();
        InitializeNativeArrays();
    }

    private void UpdateJobs()
    {
        jobHandle.Complete();
        UpdateHandlers();
        InitializeIt();
        ScheduleJob();
    }

    private void InitializeHandlers()
    {
        if (subscribedNewHandlers.Count == 0) return;

        foreach (var item in handlers)
        {
            item.CheckSwitch(false);
        }
        handlers.Clear();
        foreach (var pair in subscribedNewHandlers)
        {
            handlers.AddRange(pair.Value);
        }
        subscribedNewHandlers.Clear();
    }

    private void InitializeNativeArrays()
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
            playerPosition = target.position,
            radius = radius
        };
    }

    private void ScheduleJob()
    {
        jobHandle = agentJob.Schedule(handlers.Count, 64);
    }

    private void UpdateHandlers()
    {
        for (int i = 0; i < handlers.Count; i++)
        {
            if (handlers[i] != null)
                handlers[i].CheckSwitch(agentStates[i]);
        }
    }

    private void DisposeNativeArrays()
    {
        if (rbPositions.IsCreated)
        {
            rbPositions.Dispose();
        }

        if (agentStates.IsCreated)
        {
            agentStates.Dispose();
        }
    }

    public void AddAgent(RBHandler agent)
    {
        if (handlers.Contains(agent)) return;

        handlers.Add(agent);
    }

    public void RemoveAgent(RBHandler agent)
    {
        handlers.Remove(agent);
    }

    public void SubscribeNewTile(List<RBTile> tiles)
    {
        foreach (var tile in tiles)
        {
            if (subscribedNewHandlers.ContainsKey(tile.ID)) return;

            subscribedNewHandlers.Add(tile.ID, tile.rbPositions);
        }
    }

    protected override void OnDestroy()
    {
        jobHandle.Complete();
        DisposeNativeArrays();
    }

    [BurstCompile]
    public struct AgentJob : IJobParallelFor
    {
        public NativeArray<float3> rbPositions;
        public NativeArray<bool> agentStates;
        public float3 playerPosition;
        public float radius;

        public void Execute(int index)
        {
            agentStates[index] = math.distance(rbPositions[index], playerPosition) <= radius;
        }
    }
}
