using UnityEngine;
using UnityEngine.Events;

public class ChangeSceneTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEnterEvent; // Sự kiện UnityEvent được gọi khi va chạm

    [Tooltip("Layer mask để lọc các collider được chấp nhận va chạm")]
    public LayerMask triggerLayers = -1; // Mặc định chấp nhận tất cả layer

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu collider nằm trong layer được chấp nhận
        if (((1 << other.gameObject.layer) & triggerLayers) != 0)
        {
            // Gọi sự kiện UnityEvent nếu được gán trong Inspector
            if (onTriggerEnterEvent != null)
            {
                onTriggerEnterEvent.Invoke();
                Debug.Log("Trigger entered by: " + other.gameObject.name + ", Event invoked.");
            }
            else
            {
                Debug.LogWarning("No event assigned to onTriggerEnterEvent in ChangeSceneManager!");
            }
        }
    }
}