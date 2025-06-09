using System.Collections;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Placeholder for SpaceShipHealthController (replace with actual implementation).
/// </summary>
public class SpaceShipHealthController : MonoBehaviour
{
    public virtual void Damaged(int damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage!");
        // Implement actual health logic here
    }
}