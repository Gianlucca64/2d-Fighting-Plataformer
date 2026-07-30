using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox_Menu : MonoBehaviour
{
    // Una referencia al botón Cargar juego
    [SerializeField] Button loadButton;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        // Siempre permitimos entrar al selector de niveles
        loadButton.interactable = true;
    }
    public void StartNewGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("Progreso borrado");

        SceneManager.LoadScene("LevelSelect");
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("LevelSelect");
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
