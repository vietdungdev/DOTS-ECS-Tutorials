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
    /// class khoi tao system manual
    /// </summary>
    public class Example_01_Init : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var world = World.DefaultGameObjectInjectionWorld;

            var findTargetSystem = world.CreateSystem<FindTargetSystem>();
            var handleTargetSystem = world.CreateSystem<HandleTargetSystem>();
            var writeLocalTransformSystem = world.CreateSystem<WriteLocalTransformSystem>();

            var simulationGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(findTargetSystem);
            simulationGroup.AddSystemToUpdateList(handleTargetSystem);
            simulationGroup.AddSystemToUpdateList(writeLocalTransformSystem);
            simulationGroup.SortSystems();
        }
    }
}