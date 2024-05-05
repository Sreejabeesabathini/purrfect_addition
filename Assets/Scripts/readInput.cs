using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

//This script allows the user to input a profile name in the profile panel.
//When they do, the user name is saved locally and in a Azure SQL DB using Serverless Function API Call

public class readInput : MonoBehaviour
{
    private string input; //the input user name
    private string deviceID; //the current user device

    // Use Awake for initialization
    void Awake()
    {
        // Now you are safely obtaining the deviceID in the Awake method
        deviceID = SystemInfo.deviceUniqueIdentifier;
    }

    //Read the username that was input, and write it to file and DB
    public void ReadStringInput(string s)
    {
        input = s;
        Debug.Log("User input name: " + input);
        WriteToFile();
    }

    private async void WriteToFile()
    {
        // Path to the file
        string currentDirectory = Application.persistentDataPath; // Assumes the code file is in the "Assets" directory
        string path = Path.Combine(currentDirectory, "userProfile.txt");
        Debug.Log($"File Path: {path}");

        // Create a file to write to or append if it already exists
        using (StreamWriter sw = new StreamWriter(path, true))
        {
            sw.WriteLine(deviceID + "," + input);
        }

        //Save the username in the Azure SQL DB using Serverless Function 
        await SendDataToAPI();

        // Log to debug that the file has been written
        Debug.Log("File written with Device ID and User Input.");
    }

    //This function send the user name information along with their device id to 
    //the Azure SQL DB being used for this project. It does this by using an
    //AZURE SERVERLESS FUNCTION API call
    private async Task SendDataToAPI()
    {
        //Getting the api call information, for user name and device id 
        string code = "JxhjZHx_aKZsMmul6hr5NyxikZoxw-4M2uHi4VRD0ke_AzFuJ0rrXQ==&clientId=default";
        string url = $"https://mathgame0305.azurewebsites.net/api/game/create?code={code}&device_id={deviceID}&username={input}";

        //Sending data to the DB
        using (HttpClient client = new HttpClient())
        {
            //Using POST call
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            //Check to see if a response is sent back
            Debug.Log("Progress data sent: " + responseString);

            //If the POST call was successful
            if (response.IsSuccessStatusCode)
            {
                responseString = await response.Content.ReadAsStringAsync();
                Debug.Log("Progress data sent: " + responseString);
            }
            //If there was an issue with the POST call
            else
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                Debug.LogError($"Failed to send progress data: {response.StatusCode} - {errorResponse}");
            }
        }

    }
}