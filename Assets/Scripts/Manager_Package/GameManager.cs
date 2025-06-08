using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void ChangeToScene(string name)
    {
        SceneManager.LoadScene(name);
    }    
    public void ExitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }    
}
