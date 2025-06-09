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
        public int currentLevel = 1;
    }

    public static SaveLoadDataManager Instance { get; private set; }
    private string saveFilePath;
    private PlayerData playerData = new PlayerData();
    private bool _isLoadData = false;
    public bool IsLoadData { get { return _isLoadData; } }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadData();
    }

    public void SaveData()
    {
        try
        {
            string jsonData = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(saveFilePath, jsonData);
            Debug.Log("Data saved to: " + saveFilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to save data: " + e.Message);
        }
    }

    public void LoadData()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");

        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("Save file not found, creating new file with default values.");
            SaveData();
            return;
        }

        try
        {
            string jsonData = File.ReadAllText(saveFilePath);
            if (string.IsNullOrEmpty(jsonData))
            {
                Debug.LogWarning("Save file is empty, using default values.");
                SaveData();
                return;
            }

            playerData = JsonUtility.FromJson<PlayerData>(jsonData);
            _isLoadData = true;
            Debug.Log("Data loaded from: " + saveFilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load save file: " + e.Message);
            Debug.LogWarning("Using default values.");
            playerData = new PlayerData();
            SaveData();
        }
    }

    // Getter/Setter với tự động lưu
    public float LoadBulletPower() => playerData.bulletPower;
    public void SaveBulletPower(float value)
    {
        playerData.bulletPower = value;
        SaveData();
    }

    public float LoadCooldownReduction() => playerData.cooldownReduction;
    public void SaveCooldownReduction(float value)
    {
        playerData.cooldownReduction = value;
    }

    public float LoadMovementSpeed() => playerData.movementSpeed;
    public void SaveMovementSpeed(float value)
    {
        playerData.movementSpeed = value;
    }

    public float LoadHealth() => playerData.health;
    public void SaveHealth(float value)
    {
        playerData.health = value;
    }

    public int LoadUpgradePoints() => playerData.upgradePoints;
    public void SaveUpgradePoints(int value)
    {
        playerData.upgradePoints = value;
    }

    public int LoadCurrentLevel() => playerData.currentLevel;
    public void SaveCurrentLevel(int value)
    {
        if (value >= 1 && value <= 3)
        {
            playerData.currentLevel = value;
            SaveData();
        }
        else
        {
            Debug.LogWarning($"Invalid level value ({value}). Setting to default level 1.");
            playerData.currentLevel = 1;
            SaveData();
        }
    }
}