using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthController : MonoBehaviour
{
    public float _defaultHealth = 500f;
    [SerializeField] private float _bonusHealth = 0.0f;
    private float MaxHealth => _defaultHealth + _bonusHealth;
    public float _health = 500f;
    public Material HealthFX;

    private float lastDamageTime; // Thời gian cuối cùng bị bắn
    private const float healDelay = 6f; // Thời gian chờ 6 giây để bắt đầu hồi máu
    private const float healRate = 50f; // Tốc độ hồi máu (50 máu/giây)

    public UnityEvent OnDie;

    void Start()
    {
        lastDamageTime = -healDelay; // Khởi tạo để hồi máu ngay từ đầu nếu không bị bắn
        UpdateHealthFX();
    }

    void Update()
    {
        // Kiểm tra nếu đã 6 giây trôi qua kể từ lần cuối bị bắn
        if (Time.time - lastDamageTime >= healDelay && _health < MaxHealth)
        {
            float healAmount = healRate * Time.deltaTime; // Số máu hồi trong frame
            _health = Mathf.Min(_health + healAmount, MaxHealth); // Giới hạn không vượt quá maxHealth
            UpdateHealthFX();
            Debug.Log("Hồi máu: " + healAmount + " (Tổng: " + _health + ")");
        }
    }

    public void DealDamage(int damage)
    {
        if (_health <= damage)
        {
            SetDie();
        }

        _health -= damage;
        _health = Mathf.Max(0, _health); // Đảm bảo không âm máu
        lastDamageTime = Time.time; // Cập nhật thời gian bị bắn
        UpdateHealthFX();
        //Debug.Log("Đang bị bắn, còn " + _health + " máu");
    }
    public void DealDamage(float damage)
    {
        if (_health <= damage)
        {
            SetDie();
        }    

        _health -= damage;
        _health = Mathf.Max(0, _health); // Đảm bảo không âm máu
        lastDamageTime = Time.time; // Cập nhật thời gian bị bắn
        UpdateHealthFX();
        //Debug.Log("Đang bị bắn, còn " + _health + " máu");
    }

    public void SetDie()
    {
        OnDie.Invoke();
    }    

    public void SetCurrentUpdateHealth()
    {
        // Hàm này hiện để trống, có thể dùng để đặt lại giá trị _health nếu cần
    }

    public void SetBonusHealth(float bonus)
    {
        _bonusHealth = bonus;
        _health += _bonusHealth;
        UpdateHealthFX();
    }    

    public void UpdateHealthFX()
    {
        float healthNormalized = 1 - Mathf.Clamp01(_health / MaxHealth);
        Debug.Log("Health Normalized: " + healthNormalized);
        HealthFX.SetFloat("_BloodIntensity", healthNormalized);
    }
}