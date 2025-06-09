using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class VirtualButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage; // Reference to the UI Image
    [SerializeField] private Color normalColor = Color.white; // Default color
    [SerializeField] private Color pressedColor = Color.gray; // Color when pressed
    [SerializeField] private UnityEvent onButtonPressed; // Event to trigger when button is pressed

    private bool isPressed = false; // Track button state

    // Called when a collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider has the "Player" tag and button hasn't been pressed yet
        if (other.CompareTag("Player_Hand") && !isPressed)
        {
            PressButton();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the collider has the "Player" tag and button hasn't been pressed yet
        if (other.CompareTag("Player_Hand") && isPressed)
        {
            ResetButton();
        }
    }

    // Function to handle button press
    private void PressButton()
    {
        isPressed = true; // Mark button as pressed
        if (buttonImage != null)
        {
            buttonImage.color = pressedColor; // Change image color to gray
        }
        onButtonPressed.Invoke(); // Trigger the assigned UnityEvent
    }

    // Optional: Reset button state (e.g., for reuse)
    public void ResetButton()
    {
        isPressed = false;
        if (buttonImage != null)
        {
            buttonImage.color = normalColor; // Revert to normal color
        }
    }
}