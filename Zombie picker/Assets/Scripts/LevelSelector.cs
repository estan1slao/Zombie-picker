using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button[] levelButtons;

    private void Start()
    {
        // Первый уровень всегда открыт
        levelButtons[0].interactable = true;

        for (int i = 1; i < levelButtons.Length; i++)
        {
            string key = $"Level_{i + 1}_Unlocked";
            bool isUnlocked = PlayerPrefs.GetInt(key, 0) == 1;
            levelButtons[i].interactable = isUnlocked;
        }
    }
}