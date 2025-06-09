using UnityEngine;
using Unity.FPS.Game;

public class PlayerStatusController : MonoBehaviour
{
    [Header("Player Status")]
    [SerializeField] private float bulletPowerBonus = 0.0f;       // Sức mạnh đạn (tăng sát thương và hiệu ứng)
    [SerializeField] private float cooldownReductionBonus = 0.0f;  // Thời gian hồi chiêu giảm (giây)
    [SerializeField] private float movementSpeedBonus = 0.0f;      // Tốc độ di chuyển (đơn vị/s)
    [SerializeField] private float healthBonus = 0.0f;

    [Header("Points System")]
    [SerializeField] private int upgradePoints = 0;           // Điểm nâng cấp (tăng khi tiêu diệt quái)

    [Header("SetUp")]
    [SerializeField] private WeaponController bulletSetup;
    [SerializeField] private PlayerActions playerSetup;
    [SerializeField] private PlayerHealthController playerHealthSetup;

    private SaveLoadDataManager saveLoadDataManager;          // Tham chiếu đến SaveLoadDataManager

    // Thuộc tính công khai để truy cập các chỉ số
    public float BulletPowerBonus => bulletPowerBonus;
    public float CooldownReductionBonus => cooldownReductionBonus;
    public float MovementSpeedBonus => movementSpeedBonus;
    public float HealthBonus => healthBonus;
    public int UpgradePoints => upgradePoints;

    void Awake()
    {
        // Tìm và gán tham chiếu đến SaveLoadDataManager
        saveLoadDataManager = FindObjectOfType<SaveLoadDataManager>();
        if (saveLoadDataManager != null)
        {
            // Lấy dữ liệu đã tải từ SaveLoadDataManager
            LoadSavedData();
        }
        else
        {
            Debug.LogWarning("SaveLoadDataManager not found in scene!");
        }
    }

    // Hàm tải dữ liệu từ SaveLoadDataManager
    private void LoadSavedData()
    {
        if (!saveLoadDataManager.IsLoadData)
        {
            saveLoadDataManager.LoadData();
        }

        if (saveLoadDataManager != null)
        {
            bulletPowerBonus = saveLoadDataManager.LoadBulletPower();
            cooldownReductionBonus = saveLoadDataManager.LoadCooldownReduction();
            movementSpeedBonus = saveLoadDataManager.LoadMovementSpeed();
            healthBonus = saveLoadDataManager.LoadHealth();
            upgradePoints = saveLoadDataManager.LoadUpgradePoints();

            bulletSetup.SetExtraDamage(bulletPowerBonus);
            playerSetup.SetDashCooldownBonus(cooldownReductionBonus);
            playerSetup.SetPlayerBonusSpeed(movementSpeedBonus);
            playerHealthSetup.SetBonusHealth(healthBonus);
        }
    }

    // Hàm tăng sức mạnh đạn (trừ 1 điểm khi tăng)
    public void IncreaseBulletPower()
    {
        if (upgradePoints >= 1)
        {
            bulletPowerBonus += 1f; // Tăng 1 đơn vị sức mạnh đạn
            upgradePoints -= 1;
            bulletSetup.SetExtraDamage(bulletPowerBonus);
            SaveTempData();
        }
        else
        {
            Debug.LogWarning("Not enough upgrade points to increase bullet power!");
        }
    }

    // Hàm giảm thời gian hồi chiêu (trừ 1 điểm khi tăng)
    public void IncreaseCooldownReduction()
    {
        if (upgradePoints >= 1)
        {
            cooldownReductionBonus -= 0.1f; // Giảm 0.1 giây thời gian hồi chiêu
            upgradePoints -= 1;
            playerSetup.SetDashCooldownBonus(cooldownReductionBonus);
            SaveTempData();
        }
        else
        {
            Debug.LogWarning("Not enough upgrade points to increase cooldown reduction!");
        }
    }

    // Hàm tăng tốc độ di chuyển (trừ 1 điểm khi tăng)
    public void IncreaseMovementSpeed()
    {
        if (upgradePoints >= 1)
        {
            movementSpeedBonus += 0.5f; // Tăng 0.5 đơn vị tốc độ di chuyển
            upgradePoints -= 1;
            playerSetup.SetPlayerBonusSpeed(movementSpeedBonus);
            SaveTempData();
        }
        else
        {
            Debug.LogWarning("Not enough upgrade points to increase movement speed!");
        }
    }

    public void IncreaseHealth()
    {
        if (upgradePoints >= 1)
        {
            healthBonus += 20f; // Tăng 20 đơn vị máu
            upgradePoints -= 1;
            playerHealthSetup.SetBonusHealth(healthBonus);
            SaveTempData();
        }
        else
        {
            Debug.LogWarning("Not enough upgrade points to increase movement speed!");
        }
    }

    // Hàm tăng điểm khi tiêu diệt quái
    public void AddUpgradePoints(int points)
    {
        if (points > 0)
        {
            upgradePoints += points;
            SaveTempData();
        }
    }

    // Hàm lưu dữ liệu hiện tại vào SaveLoadDataManager
    private void SaveTempData()
    {
        if (saveLoadDataManager != null)
        {
            saveLoadDataManager.SaveBulletPower(bulletPowerBonus);
            saveLoadDataManager.SaveCooldownReduction(cooldownReductionBonus);
            saveLoadDataManager.SaveMovementSpeed(movementSpeedBonus);
            saveLoadDataManager.SaveHealth(healthBonus);
            saveLoadDataManager.SaveUpgradePoints(upgradePoints);
        }
    }
}
