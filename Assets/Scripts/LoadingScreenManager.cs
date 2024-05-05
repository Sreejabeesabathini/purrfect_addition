using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//This script is for the loading screen, which is after the Introduction scene. 
//This scene disaplys a countdown timer for the loading screen, along with a symbol 
// and a changing colored square similar to the original app
public class LoadingScreenManager : MonoBehaviour
{
    public Text countdownText; //The countdown timer, changes every second
    public float countdownDuration = 3f; //The duration of the countdown
    public GameObject catGraphic; //For a cat graphic. Was removed, but here for reference
    public GameObject coloredSquare; //For the colored square that updates with the timer
    public GameObject[] additionSymbols; //For addition graphics. Were removed, but here for reference

    private Vector3 originalCatPosition; //To track the position of the cat graphic. Removed.
    private Quaternion originalCatRotation; //To track the circular orientatio of the cat graphic. Removed
    private Vector3[] originalSymbolPositions; //To track the positions of the addition symbols. Removed

    void Start()
    {
        
        originalCatPosition = catGraphic.transform.position; //Find original cat graphic position
        originalCatRotation = catGraphic.transform.rotation; //Find original cat graphic orientation
        originalSymbolPositions = new Vector3[additionSymbols.Length]; //find original addition symbol positions
        for (int i = 0; i < additionSymbols.Length; i++)
        {
            originalSymbolPositions[i] = additionSymbols[i].transform.position;
        }
        StartCoroutine(StartCountdown()); //Begin the loading screen countdown
    }

    //This begins the countdown before the game transitions the main screen
    IEnumerator StartCountdown()
    {
        float timer = countdownDuration;

        //While the timer isn't zero, update the positions of the cats and addition symbols
        //Also change the color of the square every second
        while (timer > 0f)
        {
            countdownText.text = Mathf.CeilToInt(timer).ToString();
            UpdateCatPosition();
            UpdateAdditionSymbols();
            UpdateColoredSquareColor(timer);
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        //When countdown is done, move onto the next scene in the game
        TransitionToNextScene();
    }

    //Every second, update the position of the cat slightly to add a small
    //sort of animation to the screen while the user waits
    void UpdateCatPosition()
    {
        //Sways up and down slightly
        float swayAmount = 50f;
        float swaySpeed = 2.5f;
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float swayY = Mathf.Sin(Time.time * swaySpeed * 0.7f) * swayAmount;
        catGraphic.transform.position = originalCatPosition + new Vector3(swayX, swayY, 0f);

        //tilts to the left and right slightly 
        float tiltAmount = 10f;
        float tiltSpeed = 2f;
        float tiltAngle = Mathf.Sin(Time.time * tiltSpeed) * tiltAmount;
        catGraphic.transform.rotation = originalCatRotation * Quaternion.Euler(0f, 0f, tiltAngle);
    }

    //Every second, update the position of the addition symbols slightly to add a small
    //sort of animation to the screen while the user waits
    void UpdateAdditionSymbols()
    {
        float swayAmount = 50f;
        float swaySpeed = 2f;

        // Swaying up and down for the first symbol
        float firstSwayY = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        additionSymbols[0].transform.position = originalSymbolPositions[0] + new Vector3(0f, firstSwayY, 0f);

        // Swaying left and right for the second symbol
        float secondSwayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        additionSymbols[1].transform.position = originalSymbolPositions[1] + new Vector3(secondSwayX, 0f, 0f);

        // Moving in a circle for the third symbol
        float circleRadius = 10f;
        float circleSpeed = 3f;
        float angle = Time.time * circleSpeed;
        float circleX = Mathf.Cos(angle) * circleRadius;
        float circleY = Mathf.Sin(angle) * circleRadius;
        additionSymbols[2].transform.position = originalSymbolPositions[2] + new Vector3(circleX, circleY, 0f);
    }

    //Every second, change the color of the square to emulate the original app
    void UpdateColoredSquareColor(float timeRemaining)
    {
        //float colorChangeDuration = 1f;
        float progress = 1f - (timeRemaining / countdownDuration);
        float hue = Mathf.Lerp(240, 360, progress); // Interpolate between blue (240) and red (360)
        coloredSquare.GetComponent<Image>().color = Color.HSVToRGB(hue / 360f, 1f, 1f);
    }

    //Move onto the main scene when the countdown is done
    void TransitionToNextScene()
    {
        SceneManager.LoadScene("main");
    }
}