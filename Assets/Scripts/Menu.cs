using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void OnMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void OnGameButton()
    {
        SceneManager.LoadScene(5);
    }

    public void OnLeaderBoardButton()
    {
        SceneManager.LoadScene(4);
    }

    public void OnProfileButton()
    {
        SceneManager.LoadScene(2);
    }

    public void OnSuccessButton()
    {
        SceneManager.LoadScene(3);
    }
}
