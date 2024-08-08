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
    /// class Monobehaviour dung de viet nhung thu kho thuc hien voi ecs nhu he thong skill...
    /// </summary>
    public class CharacterController : MonoBehaviour
    {
        public TeamId mTeam;
        public TeamId targetMask;

        protected Entity m_Entity;


        public BulletController prefabBulllet;

        private List<BulletController> bulletPool = new List<BulletController>();

        private float shootTime = 0;

        private Transform mTarget;

        // Start is called before the first frame update
        void Awake()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var manager = world.EntityManager;

            m_Entity = manager.CreateEntity();
            manager.SetName(m_Entity, name);

            world.EntityManager.AddComponent<LocalTransform>(m_Entity);

            world.EntityManager.AddComponentData(m_Entity, new TeamInfo
            {
                mTeam = this.mTeam
            });

            world.EntityManager.AddComponent<TargetResult>(m_Entity);
            world.EntityManager.AddComponentData(m_Entity, new TargetRequest
            {
                targets = this.targetMask
            });

            world.EntityManager.AddComponentObject(m_Entity, transform);
            world.EntityManager.AddComponentObject(m_Entity, this);
        }

        void OnDestroy()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
                return;

            var manager = world.EntityManager;
            manager.DestroyEntity(m_Entity);
        }

        public void SpawnBullet(Transform target)
        {
            if (Time.time - shootTime < 1)
                return;

            shootTime = Time.time;

            BulletController bullet = null;
            if (bulletPool.Count == 0)
                bullet = GameObject.Instantiate(prefabBulllet);
            else
            {
                bullet = bulletPool[bulletPool.Count - 1];
                bulletPool.RemoveAt(bulletPool.Count - 1);
            }

            bullet.SetData(transform, target, bulletPool);
            bullet.transform.position = transform.position;
            bullet.gameObject.SetActive(true);
        }

        public void OnDamage(float dmg)
        {
            Debug.Log($"{gameObject.name} receive damage {dmg}");
        }
    }

    [System.Flags]
    public enum TeamId
    {
        None = 0,
        TeamA = 1 << 0,
        TeamB = 1 << 1,
        TeamC = 1 << 2,
    }

    public struct TeamInfo : IComponentData
    {
        public TeamId mTeam;
    }

    public struct TargetRequest : IComponentData
    {
        public TeamId targets;
    }

    public struct TargetResult : IComponentData
    {
        public Entity mTarget;
    }
}