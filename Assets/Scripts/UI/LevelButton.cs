using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Nivel")]
    public string sceneName;

    [Header("Orden del nivel")]
    public int levelIndex;

    Button button;

    void Start()
    {
        button = GetComponent<Button>();

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        Debug.Log("UnlockedLevel = " + unlockedLevel);
        button.interactable = levelIndex <= unlockedLevel;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(sceneName);
    }
}