using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

namespace HybridTutorials.InteractionWithGameobject
{
    /// <summary>
    /// tutorial by vietdungdev
    /// tim muc tieu gan nhat theo vi tri cua entity(localtransform), chay multi thread voi burst
    /// </summary>
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
    [DisableAutoCreation]
    public partial struct FindTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TeamInfo>();
            state.RequireForUpdate<TargetRequest>();
            state.RequireForUpdate<TargetResult>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var targetQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, TeamInfo>().Build();

            var entities = targetQuery.ToEntityArray(state.WorldUpdateAllocator);
            var infos = targetQuery.ToComponentDataArray<TeamInfo>(state.WorldUpdateAllocator);
            var transforms = targetQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);

            var find = new FindNearestJob
            {
                AllInfos = infos,
                AllEntities = entities,
                AllTransforms = transforms
            };
            state.Dependency = find.ScheduleParallel(state.Dependency);
        }


        [BurstCompile]
        public partial struct FindNearestJob : IJobEntity
        {
            [ReadOnly] public NativeArray<TeamInfo> AllInfos;

            [ReadOnly] public NativeArray<LocalTransform> AllTransforms;

            [ReadOnly] public NativeArray<Entity> AllEntities;

            public void Execute(ref TargetResult result, in TargetRequest targetRequest, in LocalTransform transform)
            {
                var closestDistSq = 999f;
                var closestEntity = Entity.Null;

                for (int i = 0; i < AllTransforms.Length; i += 1)
                {
                    if (HasTarget(targetRequest.targets, AllInfos[i].mTeam) == false)
                        continue;

                    var distSq = math.distancesq(AllTransforms[i].Position, transform.Position);
                    if (distSq < closestDistSq)
                    {
                        closestDistSq = distSq;
                        closestEntity = AllEntities[i];
                    }
                }

                result.mTarget = closestEntity;
            }

            public bool HasTarget(TeamId targetMask, TeamId enemyId) => (targetMask & enemyId) == enemyId;
        }
    }
}