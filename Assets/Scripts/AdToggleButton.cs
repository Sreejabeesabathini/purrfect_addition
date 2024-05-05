using UnityEngine;
using UnityEngine.UI;

//This script is attached to a game object to interact with the 
// AdManager persistent game objet and close the ad banner at will
public class AdToggleButton : MonoBehaviour
{
    public Button toggleButton; // Reference to the button component

    void Start()
    {
        // Add listener to the button click event
        toggleButton.onClick.AddListener(ToggleAdVisibility);
    }

    void ToggleAdVisibility()
    {
        // Call the toggle function on the AdManager script
        AdManager.Instance.ToggleAdVisibility();
    }
}
