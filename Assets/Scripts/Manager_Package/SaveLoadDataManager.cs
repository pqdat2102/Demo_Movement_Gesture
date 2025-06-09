using UnityEngine;
using System.IO;

public class SaveLoadDataManager : MonoBehaviour
{
    [System.Serializable]
    private class PlayerData
    {
        public float bulletPower = 0.0f;
        public float cooldownReduction = 0.0f;
        public float movementSpeed = 0.0f;
        public float health = 0.0f;
        public int upgradePoints = 0;
        public int currentLevel = 1; // Màn chơi hiện tại (1, 2, hoặc 3)
    }

    private string saveFilePath;
    private PlayerData playerData = new PlayerData();
    private bool _isLoadData = false;
    public bool IsLoadData { get { return _isLoadData; } }

    void Awake()
    {
        // Đường dẫn file JSON trong StreamingAssets
        saveFilePath = Path.Combine(Application.streamingAssetsPath, "playerData.json");
        Debug.Log("Save in: " + saveFilePath);
        //LoadData();
    }

    // Lưu dữ liệu vào file JSON
    public void SaveData()
    {
        string jsonData = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(saveFilePath, jsonData);
        Debug.Log("Data saved to: " + saveFilePath);
    }

    // Tải dữ liệu từ file JSON
    public void LoadData()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("Save file not found, using default values.");
            SaveData(); // Tạo file mới với giá trị mặc định nếu không tồn tại
        }

        _isLoadData = true;
        string jsonData = File.ReadAllText(saveFilePath);
        playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        Debug.Log("Data loaded from: " + saveFilePath);
    }

    // Lấy/Save các chỉ số trạng thái
    public float LoadBulletPower() => playerData.bulletPower;
    public void SaveBulletPower(float value) { playerData.bulletPower = value; }

    public float LoadCooldownReduction() => playerData.cooldownReduction;
    public void SaveCooldownReduction(float value) { playerData.cooldownReduction = value; }

    public float LoadMovementSpeed() => playerData.movementSpeed;
    public void SaveMovementSpeed(float value) { playerData.movementSpeed = value; }

    public float LoadHealth() => playerData.health;
    public void SaveHealth(float value) { playerData.health = value; }

    public int LoadUpgradePoints() => playerData.upgradePoints;
    public void SaveUpgradePoints(int value) { playerData.upgradePoints = value; }

    // Lấy/Save màn chơi hiện tại
    public int LoadCurrentLevel() => playerData.currentLevel;
    public void SaveCurrentLevel(int value)
    {
        if (value >= 1 && value <= 3) // Giới hạn màn chơi từ 1 đến 3
        {
            playerData.currentLevel = value;
            SaveData();
        }
        else
        {
            Debug.LogWarning("Invalid level value. Must be between 1 and 3.");
        }
    }
}