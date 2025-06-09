using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI _currentUpgradePoints;
    public TextMeshProUGUI _currentBullet;
    public TextMeshProUGUI _currentSkillCooldown;
    public TextMeshProUGUI _currentSpeed;
    public TextMeshProUGUI _currentHealth;

    public void ChangeToScene(string name)
    {
        SceneManager.LoadScene(name);
    }    
    public void ExitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ReloadCharacterOptionUI()
    {

    }    
}
