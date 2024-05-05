using UnityEngine;
using UnityEngine.SceneManagement;

//This script is active when the user is in the score screen.
//Allows them to proceed to the progress screen, quit, or restart the game
public class OnClickProgress : MonoBehaviour
{
    public void LoadProgressScene()
    {
        // Load the "showprogress" scene
        SceneManager.LoadScene("showProgress");
    }
    public void Quit()
    {
        //Quit back to the main screen
        SceneManager.LoadScene("main");
    }
    public void Restart()
    {
        //Restart the game
        SceneManager.LoadScene("game");
    }
}