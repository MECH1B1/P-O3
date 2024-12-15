using UnityEngine;
using System.Collections;

/**
 * Unity script to control Arduino's built-in LED based on a public bool.
 * This example uses the Ardity library's SerialController for serial communication.
 */
public class SerialLEDController : MonoBehaviour
{
    public SerialController serialController;  // Reference to the SerialController component
    public bool ledState = false;  // Public bool to control the LED (true = on, false = off)

    // Initialization
    void Start()
    {
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>(); // Get SerialController reference
        Debug.Log("Press Space to toggle LED state");
    }

    // Update is called once per frame
    void Update()
    {
        //---------------------------------------------------------------------
        // Send data
        //---------------------------------------------------------------------

        // Send '1' or '0' based on ledState
        string msg = ledState ? "1" : "0";
        Debug.Log("Sending: " + msg);
        serialController.SendSerialMessage(msg);

        //---------------------------------------------------------------------
        // Receive data
        //---------------------------------------------------------------------

        string message = serialController.ReadSerialMessage();  // Read any incoming message from the Arduino

        if (message == null)
            return;

        // Check if the message is plain data
        Debug.Log("Message received: " + message);
    }
}
