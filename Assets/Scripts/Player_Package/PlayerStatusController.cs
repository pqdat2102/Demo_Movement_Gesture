using UnityEngine;
using Unity.FPS.Game;

public class PlayerStatusController : MonoBehaviour
{
    [Header("Player Status")]
    [SerializeField] private float bulletPower = 0.0f;       // Sức mạnh đạn (tăng sát thương và hiệu ứng)
    [SerializeField] private float cooldownReduction = 0.0f;  // Thời gian hồi chiêu giảm (giây)
    [SerializeField] private float movementSpeed = 0.0f;      // Tốc độ di chuyển (đơn vị/s)

    [Header("Points System")]
    [SerializeField] private int upgradePoints = 0;           // Điểm nâng cấp (tăng khi tiêu diệt quái)

    [Header("SetUp")]
    [SerializeField] private WeaponController bulletSetUp;
    [SerializeField] private PlayerActions playerSetUp;

    private SaveLoadDataManager saveLoadDataManager;          // Tham chiếu đến SaveLoadDataManager

    // Thuộc tính công khai để truy cập các chỉ số
    public float BulletPower => bulletPower;
    public float CooldownReduction => cooldownReduction;
    public float MovementSpeed => movementSpeed;
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
            bulletPower = saveLoadDataManager.LoadBulletPower();
            cooldownReduction = saveLoadDataManager.LoadCooldownReduction();
            movementSpeed = saveLoadDataManager.LoadMovementSpeed();
            upgradePoints = saveLoadDataManager.LoadUpgradePoints();

            bulletSetUp.SetExtraDamage(bulletPower);
            playerSetUp.SetDashCooldownBonus(cooldownReduction);
            playerSetUp.SetPlayerBonusSpeed(movementSpeed);
        }
    }

    // Hàm tăng sức mạnh đạn (trừ 1 điểm khi tăng)
    public void IncreaseBulletPower()
    {
        if (upgradePoints >= 1)
        {
            bulletPower += 1f; // Tăng 1 đơn vị sức mạnh đạn
            upgradePoints -= 1;
            bulletSetUp.SetExtraDamage(bulletPower);
            SaveCurrentData();
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
            cooldownReduction -= 0.1f; // Giảm 0.1 giây thời gian hồi chiêu
            upgradePoints -= 1;
            playerSetUp.SetDashCooldownBonus(cooldownReduction);
            SaveCurrentData();
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
            movementSpeed += 0.5f; // Tăng 0.5 đơn vị tốc độ di chuyển
            upgradePoints -= 1;
            playerSetUp.SetPlayerBonusSpeed(movementSpeed);
            SaveCurrentData();
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
            SaveCurrentData();
        }
    }

    // Hàm lưu dữ liệu hiện tại vào SaveLoadDataManager
    private void SaveCurrentData()
    {
        if (saveLoadDataManager != null)
        {
            saveLoadDataManager.SaveBulletPower(bulletPower);
            saveLoadDataManager.SaveCooldownReduction(cooldownReduction);
            saveLoadDataManager.SaveMovementSpeed(movementSpeed);
            saveLoadDataManager.SaveUpgradePoints(upgradePoints);
        }
    }
}
