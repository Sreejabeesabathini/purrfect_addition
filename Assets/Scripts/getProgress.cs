using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;

//This script is what calls the last 5 games the chosen user name has played on the
// current device. It is called when the user chooses the progress screen button 
// on the score screen
public class DisplayLastFiveScores : MonoBehaviour
{
    public TextMeshProUGUI scoreTableText;      //To host the score text onto a panel.
    public TextMeshProUGUI showName;            //Will output the name of the current user
    public TextMeshProUGUI showCorrectAnswers;  //Will output the correct answer amount from last 5 games
    public TextMeshProUGUI showAccuracy;        //Outputs accuracy percentage
    public TextMeshProUGUI showRate;            //Outputs rate of questions answered
    private string userName; //Current user name
    private List<string[]> matchingData; //Holds line from .txt files for searching


    //Begin displaying last 5 scores for current user
    private void Start()
    {
        DisplayScores();
    }

    //Fetches the last 5 scores
    //Uses a local .txt file to save locally in case any issues writing to the server occur in previous scene
    private void DisplayScores()
    {
        //Reads a text file with the following name userProgress.txt
        string filePath = Path.Combine(Application.persistentDataPath, "userProgress.txt");

        //Proceeds when it finds the locally stored .txt file 
        if (File.Exists(filePath))
        {
            //Gets the deviceId and current path, also finds the path to the .txt file
            string deviceID = SystemInfo.deviceUniqueIdentifier;
            string currentDirectory = Application.persistentDataPath;
            string userProfilePath = Path.Combine(currentDirectory, "userProfile.txt");
            //Calls seperate function to retrieve the user name
            GetUserName(userProfilePath, deviceID);

            //Lists all lines in progress .txt file
            var lines = File.ReadAllLines(filePath);


            if (userName == "KittenMath")
            {
                matchingData = lines
                .Select(line => line.Split(','))
                .Where(data => data.Length > 1 && data[0].Trim() == deviceID)
                .Reverse() // Reverse to get the last entries first
                .Take(5) // Take only the last 5 entries
                .Reverse() // Reverse again to display them in the original order
                .ToList();
            }
            else
            {

                // - uncomment if data needs to be filtered by userName and deviceId
                matchingData = lines
                .Select(line => line.Split(','))
                .Where(data => data.Length > 2 && data[0].Trim() == deviceID && data[1].Trim() == userName)
                .Reverse() // Reverse to get the last entries first
                .Take(5) // Take only the last 5 entries
                .Reverse() // Reverse again to display them in the original order
                .ToList();
            }


            /*
            var matchingData = lines
            .Select(line => line.Split(','))
            .Where(data => data.Length > 1 && data[0].Trim() == deviceID)
            .Reverse() // Reverse to get the last entries first
            .Take(5) // Take only the last 5 entries
            .Reverse() // Reverse again to display them in the original order
            .ToList();
            */

            //Display user name in scene
            showName.text = userName;

            //Disaplys the other three variables, correct answers, accuracy, rate
            if (matchingData.Any())
            {
                showCorrectAnswers.text = ""; //initialize correct answers
                showAccuracy.text = ""; //initialize accuracy
                showRate.text = ""; //initialize rate

                //begin displaying data per row
                foreach (var record in matchingData)
                {
                    // scoreTableText.text += $"{record[2]} | {record[3]}% | {record[4]}/min\n";
                    Debug.Log("records: " + record[3]);
                    showCorrectAnswers.text += $"{record[3]}\n";
                    showAccuracy.text += $"{record[4]}%\n";
                    showRate.text += $"{record[5]}/min\n";
                }
            }
            //If record for current user is not found for any reason
            else
            {
                Debug.LogWarning("No records found for the device ID in 'userProgress.txt'.");
                scoreTableText.text = "Error: No data found";
            }
        }
        //If the .txt file is not found for any reason
        else
        {
            Debug.LogWarning("File not found.");
            scoreTableText.text = "Error: File not found";
        }
    }

    //Find the user name in the progress .txt file, passed on from a different file
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

            Debug.Log("User name found in getProgress: " + userName);
        }
        else
        {
            Debug.LogError("The 'userProfile.txt' file does not exist.");
        }

        // At this point, userName is either the name found in the file or "null"
    }
}