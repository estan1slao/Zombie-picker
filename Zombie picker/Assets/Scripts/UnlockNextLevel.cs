using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockNextLevel : MonoBehaviour
{
    public int nextLevel = 1;
    void OnEnable()
    {
        string key = $"Level_{nextLevel}_Unlocked";
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }
}
