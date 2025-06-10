using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIContinueController : MonoBehaviour
{
    [SerializeField] private int _level;
    public void Start()
    {
        _level = FindAnyObjectByType<SaveLoadDataManager>().GetSavedLevel();
        if (_level == 0)
        {
            GetComponent<Image>().color = Color.gray;
        }    
            
    }
}
