using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperCharacter : MonoBehaviour
{
    public Transform target;               // Đối tượng sẽ di chuyển
    public List<Transform> points;         // Danh sách các vị trí đích
    public float speed = 6f;               // Tốc độ di chuyển
    public bool isUse = false;
    private int currentPoint = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!isUse && other.transform == target)
        {
            isUse = true;
            other.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    void Update()
    {
        if (!isUse) return;
        if (target == null || points.Count == 0) return;

        Transform point = points[currentPoint];
        target.position = Vector3.MoveTowards(target.position, point.position, speed * Time.deltaTime);

        // Khi đến gần vị trí đích, chuyển sang điểm tiếp theo
        if (Vector3.Distance(target.position, point.position) < 0.05f)
        {
            currentPoint++;
            if (currentPoint >= points.Count)
            {
                isUse = false;
                target.GetComponent<Rigidbody>().useGravity = true;
                currentPoint = points.Count - 1; // Dừng lại ở điểm cuối cùng
            }
        }
    }


}
