using System.Collections;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Placeholder for SpaceShipHealthController (replace with actual implementation).
/// </summary>
public class SpaceShipHealthController : MonoBehaviour
{
    [Header("Player Setting")]
    public UnityEvent<int> OnDamagedPlayer;
    public bool _isPlayer;

    [Header("Enemy Setting")]
    public float _maxHealth = 500;
    public float _currentHealth = 500;
    public UnityEvent OnDamagedEnemy;

    [Header("Behavior Setting")]
    public UnityEvent OnDie;

    public virtual void Damaged(int damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage!");
        // Implement actual health logic here
        if (_isPlayer)
        {
            OnDamagedPlayer.Invoke(damage);
        }
        else
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
               Die();
            }
        } 
    }

    public void Die()
    {
        OnDie.Invoke();
        Destroy(gameObject, 0.0f);
    }    
}