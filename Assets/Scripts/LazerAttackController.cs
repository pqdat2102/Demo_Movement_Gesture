using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerAttackController : MonoBehaviour
{

    [SerializeField] private Transform player; // Transform của người chơi
    [SerializeField] private float attackDuration = 2f; // Thời gian tấn công (giây)
    [SerializeField] private float moveSpeed = 2f; // Tốc độ di chuyển của TargetObject
    [SerializeField] private float attackCooldown = 4f; // Thời gian chờ giữa các lần tấn công
    [SerializeField] private float attackRange = 5f; // Phạm vi tấn công
    [SerializeField] private float damageRange = 10f; // Khoảng cách tối đa từ targetCursor để gây sát thương
    [SerializeField] private float damageInterval = 2f; // Khoảng thời gian giữa các lần gây sát thương (giây)
    public SFXControllerV3D sfgx;
    public ProgressControlV3D progress;
    public Transform targetObject;

    private Transform targetCursor;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private float cooldownTimer = 0f;
    private float damageTimer = 0f;

    private void Start()
    {
        targetCursor = transform.GetComponent<MouseTargetV3D>().targetCursor;
    }

    private void Update()
    {
        HandleAttackBehavior();
    }

    private void HandleDamageBehavior()
    {
        // Kiểm tra xem người chơi và targetCursor có tồn tại không
        if (player == null || targetCursor == null)
            return;

        // Kiểm tra khoảng cách giữa người chơi và targetCursor
        if (Vector3.Distance(player.position, targetCursor.position) <= damageRange)
        {
            // Người chơi trong phạm vi, đếm thời gian để gây sát thương
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                DealDamagePlayer();
                damageTimer = 0f; // Reset timer sau khi gây sát thương
            }
        }
        else
        {
            // Người chơi ngoài phạm vi, reset timer
            damageTimer = 0f;
        }
    }

    private void DealDamagePlayer()
    {
        transform.GetComponent<PlayerHealthController>().DealDamage(10);
    }

    private void HandleAttackBehavior()
    {
        // Kiểm tra xem người chơi có trong phạm vi tấn công không
        if (player != null && Vector3.Distance(transform.position, player.position) > attackRange)
        {
            // Người chơi ngoài phạm vi, không tấn công
            if (isAttacking)
            {
                isAttacking = false;
                AttackPlayer(false);
            }
            return;
        }

        if (!isAttacking)
        {
            // Đếm thời gian chờ giữa các lần tấn công
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= attackCooldown)
            {
                // Bắt đầu tấn công
                isAttacking = true;
                AttackPlayer(true);
                attackTimer = 0f;
            }
        }
        else
        {
            // Đếm thời gian tấn công
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                // Kết thúc tấn công
                isAttacking = false;
                AttackPlayer(false);
                cooldownTimer = 0f;
            }

            // Di chuyển TargetObject về phía người chơi
            if (targetObject != null && player != null)
            {
                Vector3 targetPosition = player.position;
                targetObject.position = Vector3.Lerp(
                    targetObject.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );
            }
        }
    }

    private void AttackPlayer(bool state)
    {
        sfgx.Attack(state);
        progress.Attack(state);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (targetCursor != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetCursor.position, damageRange);
        }
    }


}
