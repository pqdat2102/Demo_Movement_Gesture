using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Level")]
    public int _level = 0;
    [Header("Character UI")]
    public TextMeshProUGUI _currentUpgradePoints;
    public TextMeshProUGUI _currentBullet;
    public TextMeshProUGUI _currentSkillCooldown;
    public TextMeshProUGUI _currentSpeed;
    public TextMeshProUGUI _currentHealth;
    public PlayerStatusController status;

    public void NextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;
        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(0);
            Debug.LogError("Lỗi không có scenen tương ứng, trở về menu chính");
            return;
        }    

        FindAnyObjectByType<SaveLoadDataManager>().SaveCurrentLevel(_level);
        SceneManager.LoadScene(nextIndex);
    }    
    public void ExitToMenu()
    {
        SceneManager.LoadScene("Main");
    }

    public void ReloadCharacterUI()
    {
        _currentUpgradePoints.text = status.UpgradePoints.ToString();
        _currentBullet.text = status.BulletPowerBonus.ToString();
        _currentSkillCooldown.text = status.CooldownReductionBonus.ToString();
        _currentSpeed.text = status.MovementSpeedBonus.ToString();
        _currentHealth.text = status.HealthBonus.ToString();
    }    
}
