using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonManager : MonoBehaviour
{
  
    public void RestartGame()
    {
        SceneManager.LoadScene("Base");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
