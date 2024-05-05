using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Import SceneManager to handle scene changes
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System;
using System.IO; // Added for file I/O
using ClassLibrary1;//Implementing NUGET package for maths addition
using System.Security.Cryptography;
using System.Net.Http;
using System.Threading.Tasks;
using Pcg;//Implementing NUGET package for random number generator

//This script manages the main game functionality. It runs when the play button in the main screen is tapped.
//It generates a random addition math question for the user, along with three answer buttons. Two of these buttons
//contain the wrong answer, one the right answer. The question is graphically represented by two colored cat variants
//in red and blue. When tapping an answer button, the user is given positive or negative feedback depending 
//on whether the correct or incorrect answer was chosen.
//The game finished when the user answers the target number of questions indicated by the score counter

public class MathGame : MonoBehaviour
{
    public Text questionText; //holds the text for the current question, older variable
    public Button[] answerButtons; //all three of the answer buttons on the bottom
    public Text questionCounterText; //holds text for current question, newer variable
    public GameObject Blurbackground; //GameObject that allows blurring of background
    public GameObject pauseMenu; // Add reference to the pause menu panel
    public GameObject correctAnswerPrompt; //GameObject that is active when the correct answer is chosen
    public GameObject wrongAnswerPrompt; //GameObject that is active when incorrect answer is chosen
    public GameObject problemQuestionCanvas; //GameObject that hosts the question text displayed
    public GameObject[] catUnits; //GameObjects representing the Cats on the screen
    public Vector3[] startPositions; // Animation Functionality (off screen position)
    public Vector3[] targetPositions; //Animation Functionality (final screen position)
    public float speed = 0f; // Animation Functionality: Speed at which the cat moves
    public GameObject catContainer; // Animation Functionality: Assign this in the Inspector
    public AudioSource correctAnswerSound; //Happy audio voice, on correct answer
    public AudioSource wrongAnswerSound; //Sad audio voice, on incorrect answer
    public AudioSource correctSound; //Happy sound, on correct answer
    public AudioSource wrongSound; //Sad sound, on incorrect answer
    public GameObject[] allCats; //New Objects representing updated Cat models
    public GameObject HappyCat_0; //Object representing happy cat animation
    public GameObject SadCat_1; //Object representing sad cat animation


    private Button correctButton; // Correct answer button
    private Color defaultButtonColor = Color.white; // The default color for buttons
    private Color correctButtonColor = Color.green; // The color for correct answers
    private Color incorrectButtonColor = Color.red; // The color for incorrect answers
    private Vector3 offScreenPosition = new Vector3(-10f, 0f, 0f); // Animation Functionality: Example off-screen position

    private int questionCounter = 0; //Keeps track of total questions asked so far
    private bool quizCompleted = false; //Flag to indicate if game is finished
    private bool gamePaused = false; // Add variable to track game pause state
    private int correctAnswers = 0; //Total correct answers
    private int totalQuestions = 15; //Target number of questions
    private float rate; //Rate of how fast questions were answers / minutes
    private float accuracy; //Accuracy percentage of correct answers
    private float startTime; //Time when starting game
    private float endTime;  //Time when finishing current game
    private float totalTime; //Total time spent in current run
    private string userName; //User name of current user
    private bool buttonsRespondingToInput = true; // Flag to track whether buttons should respond to input
    private PcgRandom randomGenerator; //NUGET package library used to generate random numbers at indicated range

    public AudioSource backgroundMusic; // Reference to the background music AudioSource
    public Button musicToggleButton; // Reference to the button that toggles the background music
    public Sprite musicOnSprite; // Sprite to display when music is on
    public Sprite musicOffSprite; // Sprite to display when music is off

    private bool isMusicOn = true; // Flag to track if music is currently playing
    private float audioPausedTime; // Time at which audio was paused

    // Array to hold the prerecorded voice clips for numbers 0 through 6, 'plus', and 'equals'
    public AudioSource[] numberAudioSources;
    private bool answerSelected = false; // Flag variable to indicate whether an answer has been selected


    void Start()
    {
        randomGenerator = new PcgRandom(); // Initialize the PcgRandom generator from Nuget package
        InitializeCats(); //Initialize the Cat Units used for the question
        GenerateQuestion(); //Generate random addition question
        UpdateMusicButtonSprite(); // Set the initial sprite and color of the button

    }

    //To display the cats during the problems on the screen
    void DisplayCatsForQuestion(int num1, int num2)
    {
        // Deactivate all cats initially
        foreach (GameObject cat in catUnits)
        {
            cat.SetActive(false);
        }

        // Calculate the index offsets for each row
        int row1Offset = 0;       // Start of the first row
        int row2Offset = 4;       // Start of the second row
        int row3Offset = 8;       // Start of the third row

        // We need to distribute num1 over the first two rows (8 cats max)
        int num1FirstRow = Mathf.Min(num1, 4);  // Cats in the first row
        int num1SecondRow = Mathf.Max(0, num1 - 4);  // Remaining cats in the second row

        // Activate cats based on num1 for the first row
        for (int i = 0; i < num1FirstRow; i++)
        {
            catUnits[row1Offset + i].SetActive(true);
        }

        // Activate cats based on remaining num1 for the second row
        for (int i = 0; i < num1SecondRow; i++)
        {
            catUnits[row2Offset + i].SetActive(true);
        }

        // Activate cats based on num2 for the third row
        for (int i = 0; i < num2 && i < 4; i++)
        {
            catUnits[row3Offset + i].SetActive(true);
        }
    }


    //Generate a random math question using NUGET math functions
    void GenerateQuestion()
    {

        if (!quizCompleted && !gamePaused) // Check if the quiz is not completed and the game is not paused
        {
            HappyCat_0.SetActive(false); //Deactivate happy cat animation
            SadCat_1.SetActive(false); //Deactivate sad cat animation
            
            // Increment question counter
            questionCounter++;

            // Generate random numbers for the addition question from Pcg NUGET package
            int num1 = randomGenerator.Next(1, 6); // Generates a random number between 1 and 6 
            int num2 = randomGenerator.Next(1, 3); // Generates a random number between 1 and 3

            // Instantiate BasicMathsFunctions from ClassLibrary1NUGET package
            BasicMathsFunctions math = new BasicMathsFunctions(); 
            int answer = (int)math.Addition(num1, num2); // Call the Addition method

            //DisplayCatsForQuestion(num1, num2);
            ActivateCats(num1, num2);

            //Check if target number of questions has been reached
            if (questionCounter <= totalQuestions)
            {
                //Update question counter
                questionCounterText.text = $"{questionCounter} / {totalQuestions}";
                // Display the question
                questionText.text = num1 + " + " + num2 + "= ?";

                answerSelected = false; // reset flag variable after every answer

                // Example: Play audio for numbers num1 and num2
                PlayMathQuestionAudio(num1, num2);
            }
            else
            { 
                quizCompleted = true;
                LoadShowScoreScene(); //Load the "showScore" scene
            }

            // Reset the flag to allow button input for the new question
            SetButtonsRespondingToInput(true);

            // List to store wrong answers
            List<int> wrongAnswers = new List<int>();

            // Generate random answer options using Pcg NUGET package
            int correctButtonIndex = randomGenerator.Next(0, answerButtons.Length);

            for (int i = 0; i < answerButtons.Length; i++)
            {
                //Activate answer buttons if target goal hasn't been reached yet
                if (questionCounter <= totalQuestions)
                {
                    answerButtons[i].gameObject.SetActive(true); // Show answer buttons for first 3 questions
                }
               

                //If this answer button is the one denoted as the correct answer
                if (i == correctButtonIndex)
                {
                    answerButtons[i].GetComponentInChildren<Text>().text = answer.ToString(); //get correct answer text
                    answerButtons[i].onClick.RemoveAllListeners(); // Remove previous listeners
                    correctButton = answerButtons[i]; //set this button as the correct one
                    answerButtons[i].onClick.AddListener(CorrectAnswer); //add listener for correct answers
                }
                else
                {
                    // Generate random incorrect answer options using Pcg NUGET package
                    // Change the range as per your requirement
                    int wrongAnswer = randomGenerator.Next(answer / 2, answer + 3); 
                    while (wrongAnswers.Contains(wrongAnswer) || wrongAnswer == answer)
                    {
                        // Generate random incorrect answer options using Pcg NUGET package
                        wrongAnswer = randomGenerator.Next(answer / 2, answer + 3);
                    }
                    wrongAnswers.Add(wrongAnswer); // Add wrong answer to the list
                    answerButtons[i].GetComponentInChildren<Text>().text = wrongAnswer.ToString(); //add incorrect answer text
                    answerButtons[i].onClick.RemoveAllListeners(); // Remove previous listeners
                    answerButtons[i].onClick.AddListener(WrongAnswer); //add listener for the incorrect answer(s)
                }
            }

            //Check if target goal reached
            if (questionCounter > totalQuestions)
            {
                quizCompleted = true;
                endTime = Time.time; // Record the end time when the quiz is completed
                totalTime = endTime - startTime; // Calculate the total time taken
                accuracy = ((float)correctAnswers / totalQuestions) * 100;
                accuracy = Mathf.Round(accuracy * 100) / 100; // Round accuracy to two decimal places
                rate = (totalQuestions / totalTime) * 60f;

                // Assumes the code file is in the "Assets" directory
                string currentDirectory = Application.persistentDataPath; 
                string filePath = Path.Combine(currentDirectory, "showScore.txt");
                Debug.Log($"File Path: {filePath}");

                //Assumes the profile file is in the "Assets" directory
                string userProfilePath = Path.Combine(currentDirectory, "userProfile.txt");
                string deviceID = SystemInfo.deviceUniqueIdentifier;
                
                //Find username of current user on this device
                GetUserName(userProfilePath, deviceID);

                //Stats to be written to file
                string csvContent = $"{totalQuestions},{correctAnswers},{accuracy:F2},{rate:F2}";

                //try to list all lines in the score files
                try
                {
                    File.WriteAllText(filePath, csvContent);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error writing to file: {e.Message}");
                }

                // Code for user progress

                string currentDirectory1 = Application.persistentDataPath; // Assumes the code file is in the "Assets" directory
                string filePath1 = Path.Combine(currentDirectory1, "userProgress.txt");
                Debug.Log($"File Path: {filePath1}");
                Debug.Log("Device ID: " + deviceID);

                //Information to be written to file
                string csvContent1 = $"{deviceID},{userName},{totalQuestions},{correctAnswers},{accuracy:F2},{rate:F2}\n"; // Add newline character

                try
                {
                    File.AppendAllText(filePath1, csvContent1); // Appends text to the file, creating the file if it doesn't already exist.
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error writing to file: {e.Message}");
                }

                //Send data to Azure SQL DB using AZURE SERVERLESS FUNCTION API call
                SendProgressData();
            }
        }
    }

    //This function send the user name, device id, correct answers, accuracy, and rate information
    //to the Azure SQL DB being used for this project. It does this by using an
    //AZURE SERVERLESS FUNCTION API call
    private async Task SendProgressData()
    {
        //Getting the api call information, for user name, device id, correct answers, accuracy, rate 
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        string code = "JxhjZHx_aKZsMmul6hr5NyxikZoxw-4M2uHi4VRD0ke_AzFuJ0rrXQ==&clientId=default";
        string url = $"https://mathgame0305.azurewebsites.net/api/game/progress?code={code}&device_id={deviceID}&username={userName}&questions={totalQuestions}&correct_answers={correctAnswers}&accuracy={accuracy:F2}&rate={rate:F2}";

        //Sending data to the DB
        using (HttpClient client = new HttpClient())
        {
            //Using POST call
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            //Response is sent back
            string responseString = await response.Content.ReadAsStringAsync();

            //If a successful response from POST
            if (response.IsSuccessStatusCode)
            {
                responseString = await response.Content.ReadAsStringAsync();
                Debug.Log("Progress data sent: " + responseString);
            }
            //If an unsuccessful response from POST
            else
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                Debug.LogError($"Failed to send progress data: {response.StatusCode} - {errorResponse}");
            }
        }
    }

    public void LoadShowScoreScene()
    {
        SceneManager.LoadScene("showScore"); // Load the scene with the name "showScore"
    }


    //Activates when the user selects the wrong answer
    void CorrectAnswer()
    {
        if (!buttonsRespondingToInput)
            return;
        Debug.Log("Correct!");
        correctAnswers++; //Incrememnts correct answer score

        //Highlights the correct answer button with green
        HighlightButton(correctButton, correctButtonColor);

        // Stop the prerecorded voice clip for the addition question when the player selects an answer
        AnswerSelected();

        //Starts the correct answer prompt for two seconds to allow for feedback
        StartCoroutine(ShowPrompt(correctAnswerPrompt));
        correctAnswerSound.Play(); //happy voice response
        correctSound.Play(); //happy sound feedback
        buttonsRespondingToInput = false; // Disable further button input
        HappyCat_0.SetActive(true); //play happy cat animation
    }

    //Activates when the user selects the wrong answer
    void WrongAnswer()
    {
        if (!buttonsRespondingToInput)
            return;
        Debug.Log("Wrong!");
        HighlightButton(correctButton, correctButtonColor); //highlight the correct answer for the user
        //Indicate the button(s) with the incorrect answers
        Button incorrectButton = (Button)UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        
        //highlight them with a different color to indicate they are wrong
        HighlightButton(incorrectButton, incorrectButtonColor); 
        
        // Stop the prerecorded voice clip for the addition question when the player selects an answer
        AnswerSelected();

        //Start the wrong answer prompt temporarily, for two second (to allow feedback)
        StartCoroutine(ShowPrompt(wrongAnswerPrompt));
        wrongAnswerSound.Play(); //sad voice clip response
        wrongSound.Play(); //sad sound response
        buttonsRespondingToInput = false; // Disable further button input
        SadCat_1.SetActive(true); //play sad cat animation
    }

    // Helper method to enable/disable button input
    void SetButtonsRespondingToInput(bool responding)
    {
        buttonsRespondingToInput = responding;
    }

    //Change the color of a button temporarily
    void HighlightButton(Button button, Color color)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = color;
            StartCoroutine(ResetButtonColor(buttonImage));
        }
    }

    //Reset the answer button colors after answering
    IEnumerator ResetButtonColor(Image buttonImage)
    {
        if (quizCompleted == true)
        {
            yield return new WaitForSeconds(0.0f);
        }
        yield return new WaitForSeconds(1.0f); // Adjust the delay time as needed
        buttonImage.color = defaultButtonColor; //go back to default color scheme
    }

    // Helper method to set interactable state of an array of buttons
    private void SetButtonsInteractable(Button[] buttons, bool interactable)
    {
        foreach (Button button in buttons)
        {
            button.interactable = interactable;
        }
    }

    //Pause the game upon hitting the pause button in the top corner
    public void PauseGame()
    {
        gamePaused = true; //set true to make sure game is paused
        pauseMenu.SetActive(true); //bring up the pause menu
        //problemQuestionCanvas.SetActive(false);
        Time.timeScale = 0f; // Effectively pauses the game
        SetButtonsInteractable(answerButtons, false); //make sure answer buttons can't be interacted with
        Blurbackground.SetActive(true); //blur the background aside from the pause menu

        // Deactivate cats directly in this method
        //foreach (GameObject cat in catUnits)
        foreach (GameObject cat in allCats)
        {
            // Directly disable the SpriteRenderer component
            SpriteRenderer catSprite = cat.GetComponent<SpriteRenderer>();
            if (catSprite != null)
            {
                catSprite.enabled = false;
            }

            // Pause animations by setting animator speed to 0
            Animator catAnimator = cat.GetComponent<Animator>();
            if (catAnimator != null)
            {
                catAnimator.speed = 0;
            }
        }
        Time.timeScale = 0f; // added for animation functionality 
    }

    //Resume the game from the pause menu
    public void ResumeGame()
    {
        gamePaused = false; //unpause the game
        pauseMenu.SetActive(false); //remove the pause menu
        problemQuestionCanvas.SetActive(true); //reset the problem canvas for questions
        Time.timeScale = 1f;
        ReactivateCats(); //reactive any cat units that were disabled during pausing
        SetButtonsInteractable(answerButtons, true); //make the answer buttons interactable again
        Blurbackground.SetActive(false); //stop blurring the background
    }

    //Reactivate the Cat units that were disabled temporarily
    void ReactivateCats()
    {
        //foreach (GameObject cat in catUnits)
        foreach (GameObject cat in allCats)
        {
            // Re-enable the SpriteRenderer component
            SpriteRenderer catSprite = cat.GetComponent<SpriteRenderer>();
            if (catSprite != null)
            {
                catSprite.enabled = true;
            }

            // Resume animations if they were paused
            Animator catAnimator = cat.GetComponent<Animator>();
            if (catAnimator != null)
            {
                catAnimator.speed = 1;
            }
        }
    }


    //Restart the game from the pause menu
    public void RestartGame()
    {
        questionCounter = 0; //reset the question counter
        quizCompleted = false; //set false so game is reset
        gamePaused = false; //unpause the game
        pauseMenu.SetActive(false); //remove the pause menu
        problemQuestionCanvas.SetActive(true); // reset question canvas to display question
        Time.timeScale = 1f;
        Blurbackground.SetActive(false); //stop blurring the background during pause

        //foreach (GameObject cat in catUnits)
        foreach (GameObject cat in allCats)
        {
            cat.SetActive(false); // Ensure cats start from a deactivated state for consistency
        }

        ReactivateCats(); // Ensure visual components are active before they're needed
        SetButtonsInteractable(answerButtons, true);
        GenerateQuestion(); // This should include activating the necessary cats
    }

    //Quit the game from the pause menu
    public void Quit()
    {
        gamePaused = false;
        pauseMenu.SetActive(false); // Hide the pause menu panel
        Time.timeScale = 1f; // Restore normal time flow

        SceneManager.LoadScene("LoadingScreen"); // Load the main menu scene
    }

    //Set the correct or incorrect answer prompts active for two seconds
    //Then disable and move onto the next question until the game ends
    IEnumerator ShowPrompt(GameObject prompt)
    {
        if (quizCompleted == true)
        {
            yield return new WaitForSeconds(0.0f);
        }
        prompt.SetActive(true);
        yield return new WaitForSeconds(1.0f); // Wait for 2 seconds
        prompt.SetActive(false);
        GenerateQuestion();
    }


    // Added animation functionality starts here 
    void Update()
    {
        //Active the cat walking animations to target destination
        for (int i = 0; i < catUnits.Length; i++)
        {
            GameObject cat = catUnits[i];
            if (!gamePaused && !quizCompleted && cat.activeSelf)
            {
                MoveCatToTarget(i);
            }
        }
    }

    //Start moving the Cats to their target positions
    public void MoveCatToTarget(int catIndex)
    {
        //If for some reason, the index sent to this function is out of range of total cat units available
        if (catIndex < 0 || catIndex >= catUnits.Length || catIndex >= targetPositions.Length)
        {
            Debug.LogError("Index out of range.", this);
            return;
        }

        //Take note of position of each cat unit. Start moving it towards target position
        GameObject cat = catUnits[catIndex];
        Vector3 targetPos = targetPositions[catIndex];
        cat.transform.position = Vector3.MoveTowards(cat.transform.position, targetPos, speed * Time.deltaTime);

        //Set the walking animation active until they reach target position
        Animator animator = cat.GetComponent<Animator>();
        if (animator != null)
        {
            bool hasReachedTarget = cat.transform.position == targetPos;
            animator.SetBool("IsWalking", !hasReachedTarget);
        }
    }

    //Restart the cat animations if needed
    void ResetCatPositionsAndAnimations()
    {
        for (int i = 0; i < catUnits.Length; i++)
        {
            if (i < startPositions.Length)
            {
                catUnits[i].transform.position = startPositions[i];
                Animator animator = catUnits[i].GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("IsWalking", false);
                }
            }
        }
    }

    //Begin moving cats off the screen as needed
    public void MakeCatsWalkOff()
    {
        StartCoroutine(MoveCatsOffScreen());
    }

    //Moves each cat that is active off the screen
    //No longer used, but kept for reference
    private IEnumerator MoveCatsOffScreen()
    {
        foreach (GameObject cat in catUnits)
        {
            if (cat.activeSelf)
            {
                yield return MoveCatOffScreen(cat.transform);
            }
        }
    }

    //Used to move the cats off the screen when the question changes.
    //No longer used, but kept for refernce
    private IEnumerator MoveCatOffScreen(Transform catTransform)
    {
        Vector3 offScreenPosition = new Vector3(-10f, 0f, 0f);
        while (catTransform.position.x != offScreenPosition.x)
        {
            catTransform.position = Vector3.MoveTowards(catTransform.position, offScreenPosition, speed * Time.deltaTime);
            yield return null;
        }
        catTransform.gameObject.SetActive(false);
    }

    //Find the positions of each Cat when the question appears. 
    //No longer used, but kept for reference
    void InitializeCatsAtStartPositions()
    {
        for (int i = 0; i < catUnits.Length; i++)
        {
            if (i < startPositions.Length)
            {
                catUnits[i].transform.position = startPositions[i];
            }
            else
            {
                Debug.LogWarning($"No start position for cat at index {i}, using cat's current position.");
            }
        }
    }

    //Find Username from among the .txt files locally
    void GetUserName(string filePath, string searchDeviceID)
    {
        Debug.Log("Inside getScore");
        
        // Initialize userName as "null" to ensure it has a value even if the file doesn't exist or the ID isn't found
        userName = "KittenMath";

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            Debug.Log("Lines from userProfile: " + lines);

            // Iterate through the file from the end using a reverse for loop
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                string line = lines[i];
                // Split the data by commas
                string[] data = line.Split(',');
                if (data.Length == 2 && data[0].Trim() == searchDeviceID)
                {
                    // If the deviceID matches, set the userName variable
                    userName = data[1].Trim();
                    break; // Exit the loop after finding the match
                }
            }

            Debug.Log("User name found: " + userName);
        }
        else
        {
            Debug.LogError("The 'userProfile.txt' file does not exist.");
        }

        // At this point, userName is either the name found in the file or "null"
    }

    //Makes sure all Cats are disabled upon starting
    void InitializeCats()
    {
        foreach (GameObject cat in allCats)
        {
            cat.SetActive(false); // Start with all cats deactivated
        }
    }
    
    //Will update the Cats based on the question text
    public void UpdateCatSpawnBasedOnQuestion(string questionText)
    {
        // Example questionText: "6 + 3 = ?"
        string[] parts = questionText.Split(' ');
        int firstNumber = int.Parse(parts[0]); // This is '6' from the example
        int secondNumber = int.Parse(parts[2]); // This is '3' from the example

        ActivateCats(firstNumber, secondNumber);
    }
    void ActivateCats(int countTop, int countBottom)
    {
        InitializeCats(); // Deactivate all cats to start fresh each time

        // Activate the top row cats for the first number
        for (int i = 0; i < countTop && i < 6; i++) // Ensure it does not exceed the top row count
        {
            allCats[i].SetActive(true);
        }

        // Activate the bottom row cats for the second number
        int startBottomIndex = 6; // Start from CatAnimation_0 (6)
        for (int i = 0; i < countBottom && i < 3; i++) // Ensure it does not exceed the bottom row count
        {
            int index = startBottomIndex + i;
            if (index < allCats.Length)
            {
                allCats[index].SetActive(true); // Activates CatAnimation_0 (6) to CatAnimation_0 (8)
            }
        }
    }

    //Will pause the background music. When unpaused, will play it from the same moment it was paused
    public void ToggleBackgroundMusic()
    {
        if (isMusicOn)
        {
            backgroundMusic.Pause(); // Pause the music if it's currently playing
            audioPausedTime = backgroundMusic.time; // Save the time at which audio was paused
        }
        else
        {
            backgroundMusic.UnPause(); // Unpause the music if it's currently paused
            backgroundMusic.time = audioPausedTime; // Set the audio time to the time when it was paused
        }

        isMusicOn = !isMusicOn; // Toggle the music state
        UpdateMusicButtonSprite(); // Update button sprite and color
    }

    //Changes the music icon on the pause menu when it is tapped
    void UpdateMusicButtonSprite()
    {
        musicToggleButton.image.sprite = isMusicOn ? musicOnSprite : musicOffSprite;
    }

    //Create audio the reads out math question
    private void PlayMathQuestionAudio(int num1, int num2)
    {
        List<AudioSource> audioSources = new List<AudioSource>();

        // Add number audio sources
        audioSources.Add(GetNumberAudioSource(num1 - 1));
        audioSources.Add(numberAudioSources[6]); // 'plus'
        audioSources.Add(GetNumberAudioSource(num2 - 1));
        audioSources.Add(numberAudioSources[7]); // 'equals'

        // Start coroutine to play audio sources in sequence
        StartCoroutine(PlayAudioSourcesInSequence(audioSources.ToArray()));
    }

    // Coroutine to play audio sources in sequence
    private IEnumerator PlayAudioSourcesInSequence(AudioSource[] audioSources)
    {
        //All voice clips contained in array
        foreach (AudioSource audioSource in audioSources) 
        {
            if (answerSelected)
            {
                // If an answer has been selected, break out of the loop
                break;
            }

            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
                
                //Don't allow another voice clip to play while the current is playing
                while (audioSource.isPlaying) 
                {
                    yield return null;
                }
            }
            else
            {
                Debug.LogWarning("Invalid AudioSource or AudioClip detected.");
            }
        }
    }

    // Helper method to get AudioSource for voice clip of numbers 0 through 6
    private AudioSource GetNumberAudioSource(int number)
    {
        if (number >= 0 && number <= 6)
        {
            return numberAudioSources[number];
        }
        else
        {
            return null;
        }
    }

    // Method to set the answer selected flag
    public void AnswerSelected()
    {
        answerSelected = true;
    }
}