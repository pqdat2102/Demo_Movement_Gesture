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
    [Header("Game Manager Status")]
    public bool lockStatus = false;

    public void Awake()
    {
        var resetShader = FindAnyObjectByType<ResetShaderWhenExit>();
        if (resetShader) SceneManager.sceneLoaded += resetShader.ResetHealthFX;
    }

    public void NextLevel()
    {
        if (lockStatus) return;
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
    public void OnDie()
    {
        lockStatus = true;
        StartCoroutine(WaitToExit(4f));
    }

    public void StartNewGame()
    {
        FindAnyObjectByType<SaveLoadDataManager>().DeleteData();
        NextLevel();
    }

    IEnumerator WaitToExit(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ExitToMenu();
    }

    public void ReloadCharacterUI()
    {
        if (lockStatus) return;
        _currentUpgradePoints.text = status.UpgradePoints.ToString();
        _currentBullet.text = status.BulletPowerBonus.ToString();
        _currentSkillCooldown.text = status.CooldownReductionBonus.ToString();
        _currentSpeed.text = status.MovementSpeedBonus.ToString();
        _currentHealth.text = status.HealthBonus.ToString();
    }    

    public void ContinueGame()
    {
        var _level = GetComponent<SaveLoadDataManager>().GetSavedLevel();
        SceneManager.LoadScene(_level);
    }    

    public void MuteAudio()
    {
        var audios = FindObjectsOfType<AudioSource>();
        foreach (var au in audios)
        {
            au.enabled = false;
        }
    }

    public void UnMuteAudio()
    {
        var audios = FindObjectsOfType<AudioSource>();
        foreach (var au in audios)
        {
            au.enabled = true;
        }
    }
}
