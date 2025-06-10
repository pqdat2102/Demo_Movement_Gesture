using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetShaderWhenExit : MonoBehaviour
{
    public Material HealthFX;
    private void OnApplicationQuit()
    {
        ResetHealthFX();
    }
    public void ResetHealthFX()
    {
        float healthNormalized = 0;
        Debug.Log("Health Normalized: " + healthNormalized);
        HealthFX.SetFloat("_BloodIntensity", healthNormalized);
    }

    public void ResetHealthFX(Scene scene, LoadSceneMode mode)
    {
        ResetHealthFX();
    }
}
