using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//This script is called when the user opens the game, automatically. It will 
//display a symbol that animated to grow from the center, along with the name of the game
//before transitioning to the loading screen
public class IntroductionController : MonoBehaviour
{
    public Animator symbolAnimator; //For the animation of the central symbol that expands
    public Text appNameText; //the name of the app, "Purrfect Addition
    public Text devTeamText; //the text for the team at the bottom
    public float transitionDelay = 2f; // Delay before transitioning to the next scene

    private bool transitionStarted = false; //prevents the scene from starting to transition into the next

    void Start()
    {
        // Hide the text initially
        appNameText.enabled = false;
        devTeamText.enabled = false;

        // Initialize the ad banner
        AdManager.Instance.InitializeAd();

        // Subscribe to the animation event to trigger when the symbol animation finishes
        AnimationEvent animationEvent = new AnimationEvent();
        animationEvent.functionName = "ShowText";
        
        // Set event time to end of animation
        animationEvent.time = symbolAnimator.GetCurrentAnimatorStateInfo(0).length; 
        symbolAnimator.gameObject.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].AddEvent(animationEvent);
    }

    // Function to show the text after the symbol animation finishes
    public void ShowText()
    {
        if (!transitionStarted)
        {
            appNameText.enabled = true;
            devTeamText.enabled = true;

            // Start loading the next scene asynchronously
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LoadingScreen");
            asyncLoad.allowSceneActivation = false; // Prevent automatic scene activation

            // Wait for a delay before transitioning to the next scene
            StartCoroutine(WaitAndTransition(asyncLoad));
        }
    }

    // Coroutine to wait for the delay before transitioning to the next scene
    IEnumerator WaitAndTransition(AsyncOperation asyncLoad)
    {
        transitionStarted = true;
        yield return new WaitForSeconds(transitionDelay);

        // Allow scene activation
        asyncLoad.allowSceneActivation = true;
    }
}