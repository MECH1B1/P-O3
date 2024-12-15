using UnityEngine;
using UnityEngine.UI;

public class ArduinoController : MonoBehaviour
{
    public SerialController serialController;

    // Sliders for motor PWM values
    public Slider motor1Slider;
    public Slider motor2Slider;
    public Slider motor3Slider;
    public Slider motor4Slider;

    // Public variables for non-motor values
    public Slider solenoid1Slider;  // Controls solenoid V1
    public Slider solenoid2Slider;  // Controls solenoid V2
    public Slider servoSlider;      // Controls the servo position (S)

    // Update frequency in seconds
    public float updateInterval = 0.5f;
    private float timer = 0.0f;

    void Start()
    {
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();

        // Set slider default values (optional)
        motor1Slider.value = 0;
        motor2Slider.value = 0;
        motor3Slider.value = 0;
        motor4Slider.value = 0;
        solenoid1Slider.value = 0;  // Solenoid V1 default (0 or 1)
        solenoid2Slider.value = 0;  // Solenoid V2 default (0 or 1)
        servoSlider.value = 0;      // Servo default position (0-180)
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Periodically send PWM values to the Arduino
        if (timer >= updateInterval)
        {
            SendMotorPWMValues();
            timer = 0.0f;
        }
    }

    // Handle incoming messages from the Arduino
    public void OnMessageArrived(string message)
    {
        if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
        {
            Debug.Log("Connection established");
        }
        else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
        {
            Debug.Log("Connection attempt failed or disconnection detected");
        }
        else
        {
            Debug.Log("Message arrived: " + message);

            // Example logic: If the message doesn't start with 'U', echo it back
            if (message[0] != 'U')
            {
                Debug.Log("Recieved message does not start with U. Sending back: " + message);
                // serialController.SendSerialMessage(message);
            }
        }
    }

    public void OnConnectionEvent(bool isConnected)
    {
        Debug.Log(isConnected ? "Device connected" : "Device disconnected");
    }

    // Send motor PWM values and other variables to the Arduino
    private void SendMotorPWMValues()
    {
        // Read values from sliders
        int motor1PWM = Mathf.RoundToInt(motor1Slider.value);
        int motor2PWM = Mathf.RoundToInt(motor2Slider.value);
        int motor3PWM = Mathf.RoundToInt(motor3Slider.value);
        int motor4PWM = Mathf.RoundToInt(motor4Slider.value);

        // Read values for solenoid states and servo position
        int solenoid1State = Mathf.RoundToInt(solenoid1Slider.value);  // Should be either 0 or 1
        int solenoid2State = Mathf.RoundToInt(solenoid2Slider.value);  // Should be either 0 or 1
        int servoPosition = Mathf.RoundToInt(servoSlider.value);       // Should be between 0 and 180

        // Construct the message in the format "A:M1,M2,M3,M4,V1,V2,S"
        string message = $"A:{motor1PWM},{motor2PWM},{motor3PWM},{motor4PWM},{solenoid1State},{solenoid2State},{servoPosition}";
        Debug.Log($"Sending values: {message}");

        // Send the message to the Arduino
        serialController.SendSerialMessage(message);
    }
}
