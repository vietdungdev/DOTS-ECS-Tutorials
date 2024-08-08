using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using static Unity.Entities.SystemAPI;

namespace HybridTutorials.InteractionWithGameobject
{
    /// <summary>
    /// tutorial by vietdungdev
    /// ghi position cua gameobject vao entity(localtransform)
    /// </summary>
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
    [DisableAutoCreation]
    public partial struct WriteLocalTransformSystem : ISystem
    {
        ComponentLookup<LocalTransform> m_TransformLookup;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<Transform>();

            m_TransformLookup = state.GetComponentLookup<LocalTransform>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            m_TransformLookup.Update(ref state);

            var m_Query = QueryBuilder().WithAll<Transform>().WithAllRW<LocalTransform>().Build();
            var entities = m_Query.ToEntityArray(state.WorldUpdateAllocator);
            var transformAcessArray = m_Query.GetTransformAccessArray();

            state.Dependency = new WriteAgentTransformJob
            {
                Entities = entities,
                TransformLookup = m_TransformLookup,
            }.Schedule(transformAcessArray, state.Dependency);
        }

        [BurstCompile]
        struct WriteAgentTransformJob : IJobParallelForTransform
        {
            [ReadOnly]
            public NativeArray<Entity> Entities;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<LocalTransform> TransformLookup;

            public void Execute(int index, [ReadOnly] TransformAccess transformAccess)
            {
                Entity entity = Entities[index];

                var transform = TransformLookup[entity];
                transform.Position = transformAccess.position;
                transform.Rotation = transformAccess.rotation;
                TransformLookup[entity] = transform;
            }
        }
    }
}
