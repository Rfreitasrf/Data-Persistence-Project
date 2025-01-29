using UnityEditor;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEngine;
#endif

/*
 * It handles all button navigation between scenes and game exit.
 */
public class MenuUIHandler : MonoBehaviour
{
   
    public void StartNewScene()
    {
        SceneManager.LoadScene(1);
    
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);

    }

    public void Exit()
    {

#if UNITY_EDITOR

        EditorApplication.ExitPlaymode();
#else
        Application.Quit();

#endif

    }
}
