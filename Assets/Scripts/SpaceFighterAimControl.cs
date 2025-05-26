using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceFighterAimControl : MonoBehaviour
{
    [SerializeField] private float searchRadius = 50f; // Desired search radius for enemies
    [SerializeField] private float rotationSpeed = 5f; // Speed of rotation towards target
    [SerializeField] private LayerMask enemyLayer; // Layer for enemies
    [SerializeField] private Transform targetEnemy; // Current target enemy
    [SerializeField] private bool isAiming = false; // Track if aiming is active


    // Update is called once per frame
    private void Update()
    {
        if (isAiming)
        {
            FindClosestEnemy();
            if (targetEnemy != null)
            {
                RotateTowardsTarget();
            }
        }
    }

    // Public function to activate aiming
    public void StartAim()
    {
        isAiming = true;
    }

    // Public function to stop aiming
    public void StopAim()
    {
        isAiming = false;
        targetEnemy = null;
    }

    // Find the closest enemy based on angle to forward vector
    private void FindClosestEnemy()
    {
        targetEnemy = null;
        float smallestAngle = Mathf.Infinity;

        // Find all colliders within search radius on the enemy layer
        Collider[] enemies = Physics.OverlapSphere(transform.position, searchRadius, enemyLayer);
        foreach (Collider enemy in enemies)
        {
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToEnemy);

            // Update target if this enemy has a smaller angle
            if (angle < smallestAngle)
            {
                smallestAngle = angle;
                targetEnemy = enemy.transform;
            }
        }
    }

    // Rotate the aircraft towards the target enemy
    private void RotateTowardsTarget()
    {
        Vector3 directionToTarget = (targetEnemy.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Draw search radius in Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
