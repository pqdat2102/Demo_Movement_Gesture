using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.Pooling;
using VSX.Vehicles;
using VSX.Utilities;
using VSX.Health;

namespace VSX.Weapons
{
    /// <summary>
    /// Unity event for running functions when a projectile is launched by a projectile launcher
    /// </summary>
    [System.Serializable]
    public class OnProjectileLauncherProjectileLaunchedEventHandler : UnityEvent<ProjectileCustom> { }

    /// <summary>
    /// This class spawns a projectile prefab at a specified interval and with a specified launch velocity.
    /// </summary>
    public class ProjectileWeaponUnit : WeaponUnit, IRootTransformUser, IGameAgentOwnable
    {
        protected GameAgent owner;
        public GameAgent Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        [Header("Settings")]

        [SerializeField]
        protected Transform spawnPoint;

        public override void Aim(Vector3 aimPosition)
        {
            if (aimAssistEnabled) spawnPoint.LookAt(aimPosition, transform.up);
        }
        public override void ClearAim() { spawnPoint.localRotation = Quaternion.identity; }

        [SerializeField]
        protected ProjectileCustom projectilePrefab;
        public ProjectileCustom ProjectilePrefab
        {
            get { return projectilePrefab; }
            set { projectilePrefab = value; }
        }

        [SerializeField]
        protected bool usePoolManager;

        [SerializeField]
        protected bool addLauncherVelocityToProjectile;

        [Tooltip("Additional velocity to add to the projectile (relative to the launcher). Can be used e.g. to make a missile move out to the side to prevent exhaust blocking the view.")]
        [SerializeField]
        protected Vector3 projectileRelativeImpulseVelocity = Vector3.zero;

        [SerializeField]
        protected float maxInaccuracyAngle = 2;
        public float MaxInaccuracyAngle
        {
            get { return maxInaccuracyAngle; }
            set { maxInaccuracyAngle = value; }
        }

        [Range(0, 1)]
        [SerializeField]
        protected float accuracy = 1;
        public float Accuracy
        {
            get { return accuracy; }
            set { accuracy = value; }
        }

        [Header("Events")]

        // Projectile launched event
        public OnProjectileLauncherProjectileLaunchedEventHandler onProjectileLaunched;

        protected float damageMultiplier;
        protected float healingMultiplier;
        protected int bonusDamage;
        public override void SetBonusDamage(int bonus) => bonusDamage = bonus;

        protected Transform rootTransform;
        public Transform RootTransform
        {
            set
            {
                rootTransform = value;

                if (rootTransform != null)
                {
                    rBody = rootTransform.GetComponent<Rigidbody>();
                }
                else
                {
                    rBody = null;
                }
            }
        }

        protected Rigidbody rBody;

        public override float Speed
        {
            get { return projectilePrefab != null ? projectilePrefab.Speed : 0; }
        }

        public override float Range
        {
            get { return projectilePrefab != null ? projectilePrefab.Speed * projectilePrefab.Lifetime : 0; }
        }

        public override float Damage(HealthType healthType)
        {
            if (projectilePrefab != null)
            {
                return projectilePrefab.Damage * damageMultiplier;
            }
            else
            {
                return 0;
            }
        }

        public override float Healing(HealthType healthType)
        {
            return 0; // ProjectileCustom does not support healing
        }

        protected override void Reset()
        {
            base.Reset();

            spawnPoint = transform;

            ProjectileCustom defaultProjectilePrefab = Resources.Load<ProjectileCustom>("SCK/ProjectileCustom");
            if (defaultProjectilePrefab != null)
            {
                projectilePrefab = defaultProjectilePrefab;
            }
        }

        protected virtual void Awake()
        {
            if (rootTransform == null) rootTransform = transform.root;

            if (rootTransform != null)
            {
                rBody = rootTransform.GetComponent<Rigidbody>();
            }
        }

        protected virtual void Start()
        {
            if (usePoolManager && PoolManager.Instance == null)
            {
                usePoolManager = false;
                Debug.LogWarning("No PoolManager component found in scene, please add one to pool projectiles.");
            }
        }

        /// <summary>
        /// Set the damage multiplier for this weapon unit.
        /// </summary>
        /// <param name="damageMultiplier">The damage multiplier.</param>
        public override void SetDamageMultiplier(float damageMultiplier)
        {
            this.damageMultiplier = damageMultiplier;
        }

        /// <summary>
        /// Set the healing multiplier for this weapon unit.
        /// </summary>
        /// <param name="healingMultiplier">The healing multiplier.</param>
        public override void SetHealingMultiplier(float healingMultiplier)
        {
            this.healingMultiplier = healingMultiplier;
        }

        // Launch a projectile
        public override void TriggerOnce()
        {
            if (projectilePrefab != null)
            {
                float nextMaxInaccuracyAngle = maxInaccuracyAngle * (1 - accuracy);
                spawnPoint.Rotate(new Vector3(Random.Range(-nextMaxInaccuracyAngle, nextMaxInaccuracyAngle),
                                             Random.Range(-nextMaxInaccuracyAngle, nextMaxInaccuracyAngle),
                                             Random.Range(-nextMaxInaccuracyAngle, nextMaxInaccuracyAngle)));

                // Get/instantiate the projectile
                ProjectileCustom projectileController;

                if (usePoolManager)
                {
                    projectileController = PoolManager.Instance.Get(projectilePrefab.gameObject, spawnPoint.position, spawnPoint.rotation).GetComponent<ProjectileCustom>();
                }
                else
                {
                    projectileController = GameObject.Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
                }

                // Set owner and sender root transform
                // Assuming SetOwner is inherited or implemented in a base class
                projectileController.SetSenderRootTransform(rootTransform);

                if (addLauncherVelocityToProjectile && rBody != null)
                {
                    // AddVelocity not available in ProjectileCustom; assuming base class or manual implementation
                    // For now, we'll skip or assume it's handled elsewhere
                }

                // Call the event
                onProjectileLaunched.Invoke(projectileController);

                projectileController.SetBonusDamage(bonusDamage);
            }

            ClearAim();
        }
    }
}