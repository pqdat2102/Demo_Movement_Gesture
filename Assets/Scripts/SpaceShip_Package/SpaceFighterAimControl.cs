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
    [SerializeField] private float minAngleThreshold = 2f; // Minimum angle to 
    // Public property to check aiming state
    public bool IsAiming => isAiming;


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
        // Calculate direction to target
        Vector3 directionToTarget = (targetEnemy.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Calculate angle between current forward and target direction
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        // Stop rotation if angle is below threshold to prevent jittering
        if (angleToTarget <= minAngleThreshold)
        {
            return;
        }

        // Smoothly rotate towards target
        float t = rotationSpeed * Time.deltaTime / angleToTarget; // Normalize speed based on angle
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
    }

    // Draw search radius in Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
