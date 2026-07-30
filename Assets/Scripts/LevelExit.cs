using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelExit : MonoBehaviour
{
    [Header("Siguiente escena")]
    public string nextScene = "LevelSelect";

    [Header("Orden del nivel")]
    public int currentLevelIndex;
    public TMP_Text diamondText;
    public GameObject levelCompleteText;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        CompleteLevel();

        // Detener al jugador
        PlayerController player = other.GetComponent<PlayerController>();

        if (player.HasDiamond())
        {
            diamondText.text = "💎 Diamante encontrado";
        }
        else
        {
            diamondText.text = "◇ Diamante no encontrado";
        }

        if (player != null)
            player.FinishLevel();

        // Detener enemigos
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in enemies)
        {
            enemy.enabled = false;
        }

        StartCoroutine(FinishLevel());
    }
    System.Collections.IEnumerator FinishLevel()
    {
        levelCompleteText.SetActive(true);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(nextScene);
    }
    void CompleteLevel()
    {
        int unlockedLevel =
            PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevelIndex >= unlockedLevel)
        {
            PlayerPrefs.SetInt(
                "UnlockedLevel",
                currentLevelIndex + 1);

            PlayerPrefs.Save();
        }
    }
}