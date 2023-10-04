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
    [SerializeField] private Transform[] targets;

    [SerializeField] private List<RBHandler> handlers = new List<RBHandler>();
    private Dictionary<byte, List<RBHandler>> subscribedNewHandlers = new Dictionary<byte, List<RBHandler>>();
    [SerializeField] private List<byte> currentTilesID = new List<byte>();

    private JobHandle jobHandle;
    private AgentJob agentJob;
    private NativeArray<float3> rbPositions;
    private NativeArray<bool> agentStates;

    private bool canJob;

    private void Start()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        //if(canJob)
            UpdateJobs();
    }

    public void SetTarget(Transform _target)
    {
        targets[0] = _target;
    }

    public void SetRadius(float _radius)
    {
        radius = _radius;
    }

    public void SwitchJob(bool _canJob)
    {
        canJob = _canJob;
        if(!canJob)
            handlers.Clear();

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (targets[0] == null) return;
        Gizmos.DrawWireSphere(targets[0].position, radius);

        Gizmos.color = Color.black;

        if (targets[1] == null) return;
        Gizmos.DrawWireSphere(targets[1].position, radius);
    }

    protected void InitializeIt()
    {
        DisposeNativeArrays();
        InitializeHandlers();
        InitializeNativeArrays();
    }

    public void UpdateJobs()
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
        currentTilesID.Clear();
        foreach (var pair in subscribedNewHandlers)
        {
            handlers.AddRange(pair.Value);
            currentTilesID.Add(pair.Key);
        }
        subscribedNewHandlers.Clear();
    }

    private void InitializeNativeArrays()
    {
        rbPositions = new NativeArray<float3>(handlers.Count, Allocator.Persistent);
        agentStates = new NativeArray<bool>(handlers.Count, Allocator.Persistent);

        for (int i = 0; i < handlers.Count; i++)
        {
            rbPositions[i] = handlers[i].transform.position;
            agentStates[i] = false;
        }

        agentJob = new AgentJob
        {
            rbPositions = rbPositions,
            agentStates = agentStates,
            playerPosition = targets[0].position,
            AIPosition = targets[1].position,
            radius = radius 
        };
    }

    private void ScheduleJob()
    {
        jobHandle = agentJob.Schedule(handlers.Count, 100);
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
        public float3 AIPosition;
        public float radius;    

        public void Execute(int index)
        {
            bool state = math.distance(rbPositions[index], playerPosition) <= radius || math.distance(rbPositions[index], AIPosition) <= radius;
            agentStates[index] = state;
        }
    }
}
