using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HybridTutorials.InteractionWithGameobject
{
    /// <summary>
    /// tutorial by vietdungdev
    /// class dieu khien vien dan
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class BulletController : MonoBehaviour
    {
        private List<BulletController> mPool;
        private Transform mTarget;
        private Transform mOwner;

        private void Awake()
        {
            var body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        private void OnDisable()
        {
            mTarget = null;
        }

        //co the viet trong ECS nhung ko nam trong muc dich cua tutorial nay
        private void Update()
        {
            if (mTarget == null)
                return;

            Vector3 direct = (mTarget.position - transform.position).normalized;
            transform.forward = direct;
            transform.position += direct * 10 * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform == mOwner)
                return;

            if (other.TryGetComponent<CharacterController>(out var controller) == false)
                return;

            controller.OnDamage(100);

            Despawn();
        }

        public void SetData(Transform owner, Transform target, List<BulletController> pool)
        {
            mOwner = owner;
            mTarget = target;
            mPool = pool;
        }

        private void Despawn()
        {
            if (mPool == null)
            {
                Destroy(gameObject);
                return;
            }

            gameObject.SetActive(false);
            mPool.Add(this);
        }
    }
}