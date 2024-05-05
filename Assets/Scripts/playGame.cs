using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Intended to allow the user to play the game
//Was not used, the play button was implemented in a different method
//Kept here for reference and if needed in the future
public class playGame : MonoBehaviour
{
    // Start is called before the first frame update
    public void play()
    {
        SceneManager.LoadScene(1);
    }
}