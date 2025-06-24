using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

public class LazerAttackController : MonoBehaviour
{
    [Header("Setting")]
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

    [Header("Boss Complete")]
    public UnityEvent OnBossDie;

    private Transform targetCursor;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private float cooldownTimer = 0f;
    private float damageTimer = 0f;

    private void Start()
    {
        targetCursor = transform.GetComponent<MouseTargetV3D>().targetCursor;
        GetComponent<Health>().OnDie += HandleDie;
    }

    private void Update()
    {
        HandleAttackBehavior();
        //HandleDamageBehavior();
    }

    //private void HandleDamageBehavior()
    //{
    //    if (targetCursor == null || !isAttacking)
    //        return;

    //    // Điểm bắt đầu và kết thúc của tia laser
    //    Vector3 laserStart = transform.position;
    //    Vector3 laserEnd = targetCursor.position;
    //    Vector3 laserDirection = (laserEnd - laserStart).normalized;
    //    float laserLength = Vector3.Distance(laserStart, laserEnd);

    //    // Tìm tất cả vật thể có layer "Player"
    //    int playerLayer = LayerMask.NameToLayer("Player");
    //    Collider[] hitColliders = Physics.OverlapSphere(laserStart, laserLength + damageRange, 1 << playerLayer);

    //    foreach (Collider hitCollider in hitColliders)
    //    {
    //        Vector3 objectPos = hitCollider.transform.position;

    //        // Vector từ laserStart đến vật thể
    //        Vector3 objectVector = objectPos - laserStart;

    //        // Chiếu vector vật thể lên hướng laser
    //        float projection = Vector3.Dot(objectVector, laserDirection);

    //        // Kiểm tra xem vật thể có nằm trong vùng hình trụ của tia laser
    //        if (projection >= 0 && projection <= laserLength)
    //        {
    //            // Tính điểm gần nhất trên trục laser
    //            Vector3 closestPoint = laserStart + laserDirection * projection;

    //            // Kiểm tra khoảng cách từ vật thể đến trục laser
    //            if (Vector3.Distance(objectPos, closestPoint) <= damageRange)
    //            {
    //                damageTimer += Time.deltaTime;
    //                if (damageTimer >= damageInterval)
    //                {
    //                    DealDamagePlayer();
    //                    damageTimer = 0f;
    //                }
    //                continue;
    //            }
    //        }
    //    }

    //    if (hitColliders.Length == 0)
    //        damageTimer = 0f;
    //}

    public void DealDamagePlayer()
    {
        FindAnyObjectByType<PlayerHealthController>().DealDamage(10);
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

    private void HandleDie()
    {
        OnBossDie.Invoke();
        Destroy(gameObject);
    }    


}
