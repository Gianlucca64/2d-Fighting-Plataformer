using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    public GameObject panel;

    void Start()
    {
        Time.timeScale = 0f;
    }

    public void CloseTutorial()
    {
        Time.timeScale = 1f;
        panel.SetActive(false);
    }
}