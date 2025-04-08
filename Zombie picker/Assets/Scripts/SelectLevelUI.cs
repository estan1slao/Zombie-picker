using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevelUI : MonoBehaviour
{
    public string levelName;

    public void SelectLevel()
    {
        SceneManager.LoadScene(levelName);
    }
}
