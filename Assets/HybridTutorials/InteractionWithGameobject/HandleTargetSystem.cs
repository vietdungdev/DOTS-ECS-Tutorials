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
    /// goi sang Monobehaviour, chay o main thread, ko co Burst
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [DisableAutoCreation]
    public partial class HandleTargetSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((CharacterController characterController, in TeamInfo teamInfo, in TargetResult targetResult) =>
            {
                if (targetResult.mTarget == Entity.Null)
                    return;

                var targetTransform = EntityManager.GetComponentObject<Transform>(targetResult.mTarget);
                if (targetTransform == null)
                    return;

                //trigger your skill system
                characterController.SpawnBullet(targetTransform);

#if UNITY_EDITOR
                var color = Color.blue;
                if (teamInfo.mTeam == TeamId.TeamB)
                    color = Color.yellow;
                else if (teamInfo.mTeam == TeamId.TeamC)
                    color = Color.cyan;
                Debug.DrawLine(characterController.transform.position, targetTransform.position, color);
#endif
            }).WithoutBurst().Run();
        }
    }
}