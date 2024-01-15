using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public static bool isLoosed = false;

    public GameObject PauseMenuUI;
    public GameObject LooseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (isLoosed) return;
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(isPaused)
            {                
                Resume();
            } else
            {                
                Pause();
            }
        }
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;        
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void Loose()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        LooseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isLoosed = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
