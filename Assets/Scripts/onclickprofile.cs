using UnityEngine;
using UnityEngine.SceneManagement;

//This script plays when the user click on the profile button in the top corner 
//of the main screen. This allows them to open and close the user profile panel
public class ProfileMenu : MonoBehaviour
{
    public GameObject profileMenuPanel; //refers to the profile panel object

    private void Start()
    {
        // Ensure the profile menu panel is initially inactive
        if (profileMenuPanel != null)
        {
            profileMenuPanel.SetActive(false);
        }
    }

    public void OpenProfileMenu()
    {
        // Activate the profile menu panel when the profile button is clicked
        if (profileMenuPanel != null)
        {
            profileMenuPanel.SetActive(true);
        }
    }

    public void ExitToMainScene()
    {
        // Load the "Main" scene when the exit button is clicked
        SceneManager.LoadScene("Main");
    }
}

