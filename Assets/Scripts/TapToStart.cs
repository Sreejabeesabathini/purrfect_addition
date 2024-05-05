using UnityEngine;
using UnityEngine.SceneManagement;


//This script allows the user to tap the play button in the main screen. 
//When they do, this script will play a small sound effect and then load the game screen
public class LoadGameButton : MonoBehaviour
{
    public AudioSource playButton; //uses the play button as a sound source

    // Adjust this delay time according to the length of your audio clip
    public float delayBeforeLoad = 0.25f;

    public void LoadGameScene()
    {
        // Play the audio
        playButton.Play();

        // Delay the scene transition
        Invoke("LoadSceneWithDelay", delayBeforeLoad);
    }

    private void LoadSceneWithDelay()
    {
        // Load the "game" scene after the specified delay
        SceneManager.LoadScene("game");
    }
}