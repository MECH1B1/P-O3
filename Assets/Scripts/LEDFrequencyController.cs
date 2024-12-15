using UnityEngine;
using UnityEngine.UI;

public class LEDFrequencyController : MonoBehaviour
{
    public SerialController serialController;

    // Slider to control frequency value (0-255)
    public Slider frequencySlider;

    // Update frequency in seconds
    public float updateInterval = 0.5f;
    private float timer = 0.0f;

    void Start()
    {
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();

        Debug.Log("LED Frequency Controller Initialized.");
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Periodically send the frequency value to the Arduino
        if (timer >= updateInterval)
        {
            SendFrequencyValue();
            timer = 0.0f;
        }
    }

    // Handle incoming messages from the Arduino
    public void OnMessageArrived(string message)
    {
        Debug.Log("Message arrived: " + message);

        // Optionally, add logic to process messages from the Arduino
    }

    public void OnConnectionEvent(bool isConnected)
    {
        Debug.Log(isConnected ? "Device connected" : "Device disconnected");
    }

    // Send frequency value to the Arduino
    private void SendFrequencyValue()
    {
        // Read value from the slider
        int frequency = Mathf.RoundToInt(frequencySlider.value);

        // Construct the message in the format "A:N"
        string message = $"A:{frequency}";
        Debug.Log($"Sending frequency: {message}");

        // Send the message to the Arduino
        serialController.SendSerialMessage(message);
    }
}
