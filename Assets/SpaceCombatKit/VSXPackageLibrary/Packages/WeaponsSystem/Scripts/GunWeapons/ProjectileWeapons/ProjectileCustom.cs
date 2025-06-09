using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.Weapons
{
    /// <summary>
    /// Custom projectile that continuously scans for collision with GameObjects on the "Vehicle" layer.
    /// On collision, it destroys itself and attempts to call Damaged(int damage) on SpaceShipHealthController.
    /// </summary>
    public class ProjectileCustom : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField]
        protected int damage = 10; // Damage to apply to SpaceShipHealthController

        [Header("Movement Settings")]
        [SerializeField]
        protected float speed = 100;

        [SerializeField]
        protected MovementUpdateMode movementUpdateMode = MovementUpdateMode.FixedUpdate;

        public enum MovementUpdateMode
        {
            Update,
            FixedUpdate
        }

        [Header("Lifetime Settings")]
        [SerializeField]
        protected bool disableAfterLifetime = true;

        [SerializeField]
        protected float lifetime = 3;

        [Header("Collision Scan Settings")]
        [SerializeField]
        protected float scanRadius = 0.5f; // Radius for sphere overlap scan

        [SerializeField]
        protected float scanInterval = 0.1f; // Interval between scans (seconds)

        [SerializeField]
        protected LayerMask vehicleLayerMask = ~0; // Layer mask for vehicles (set to "Vehicle" layer in Inspector)

        protected bool detonated = false;
        public bool Detonated { get { return detonated; } }

        public UnityEvent onDetonated;

        protected Transform senderRootTransform;

        protected float lifeStartTime;

        protected virtual void OnEnable()
        {
            lifeStartTime = Time.time;
            detonated = false;
            StartCoroutine(ScanForVehicleCollision());
        }

        /// <summary>
        /// Set the sender's root transform to avoid self-collision.
        /// </summary>
        public virtual void SetSenderRootTransform(Transform senderRootTransform)
        {
            this.senderRootTransform = senderRootTransform;
        }

        /// <summary>
        /// Coroutine to continuously scan for collisions with GameObjects on the "Vehicle" layer.
        /// </summary>
        protected virtual IEnumerator ScanForVehicleCollision()
        {
            while (!detonated)
            {
                // Perform sphere overlap scan
                Collider[] colliders = Physics.OverlapSphere(transform.position, scanRadius, vehicleLayerMask);

                foreach (Collider collider in colliders)
                {
                    // Ignore if the collider is part of the sender's hierarchy
                    if (senderRootTransform != null && collider.transform.IsChildOf(senderRootTransform))
                    {
                        continue;
                    }

                    // Try to find SpaceShipHealthController in the parent hierarchy
                    Transform current = collider.transform;
                    SpaceShipHealthController healthController = null;

                    while (current != null && healthController == null)
                    {
                        healthController = current.GetComponent<SpaceShipHealthController>();
                        current = current.parent;
                    }

                    // If found, call Damaged(damage)
                    if (healthController != null)
                    {
                        healthController.Damaged(damage);
                    }

                    // Destroy the projectile and exit the scan
                    Detonate();
                    break;
                }

                yield return new WaitForSeconds(scanInterval);
            }
        }

        /// <summary>
        /// Detonate the projectile (destroy it).
        /// </summary>
        protected virtual void Detonate()
        {
            detonated = true;
            onDetonated.Invoke();
            Destroy(gameObject);
        }

        /// <summary>
        /// Update the projectile's movement.
        /// </summary>
        protected virtual void MovementUpdate()
        {
            if (detonated) return;
            transform.Translate(Vector3.forward * speed * (movementUpdateMode == MovementUpdateMode.Update ? Time.deltaTime : Time.fixedDeltaTime));
        }

        /// <summary>
        /// Disable the projectile after its lifetime.
        /// </summary>
        protected virtual void DisableProjectile()
        {
            Detonate();
        }

        protected virtual void FixedUpdate()
        {
            if (movementUpdateMode == MovementUpdateMode.FixedUpdate)
            {
                MovementUpdate();
            }
        }

        protected virtual void Update()
        {
            if (movementUpdateMode == MovementUpdateMode.Update)
            {
                MovementUpdate();
            }

            if (disableAfterLifetime)
            {
                if (Time.time - lifeStartTime > lifetime)
                {
                    DisableProjectile();
                }
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            // Visualize the scan radius in the editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, scanRadius);
        }
    }
}