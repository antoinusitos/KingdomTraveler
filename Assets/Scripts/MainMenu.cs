using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Execute(bool isQuit)
    {
        if(isQuit)
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }
}
