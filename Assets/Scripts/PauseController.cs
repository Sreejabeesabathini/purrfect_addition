using UnityEngine;

//Intended to allow the user to pause the game with a key press.
//Not used because the user won't have access to the p key on a phone
public class PauseController : MonoBehaviour
{
    public GameObject pauseMenu; //refers to the pause menu panel

    private bool isPaused = false;

    // Update is called once per frame
    void Update()
    {
        // Check if the player presses the pause button (e.g., "P" key)
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // Stop time
        pauseMenu.SetActive(true); // Show pause menu
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume time
        pauseMenu.SetActive(false); // Hide pause menu
        isPaused = false;
    }

    public void QuitGame()
    {
        // Quit the application (works in standalone builds)
        Application.Quit();
    }
}