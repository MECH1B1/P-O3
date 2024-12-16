using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoSender : MonoBehaviour
{
    public SerialController serialController;

    // Update frequency in seconds
    public float updateInterval = 0.5f;
    private float timer = 0.0f;

    public int motors12PWM;
    public int motor3PWM;
    public int motor4PWM;
    public int tactilePWM;

    public bool large = false;
    public bool hard = false;
    public bool pinch = false;
    public bool tactile = false;
    bool release = false;

    
    private float releaseTime = 0;

    void Start()
    {
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Periodically send values to the Arduino
        if (timer >= updateInterval)
        {
            if (large)
            {
                SendActuatorValues(motors12PWM, motors12PWM, motor3PWM, (tactile ? motor4PWM : 0), (pinch ? 0 : 1), (release ? 1 : 0), (hard ? 0 : 180));
            } else {
                SendActuatorValues(0, 0, 0, (tactile ? motor4PWM : 0), (pinch ? 0 : 1), (release ? 1 : 0), (hard ? 0 : 180));
            }
            timer = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            release = true;
            releaseTime = Time.time;
        }

        if (releaseTime + updateInterval <= Time.time)
        {
            release = false;
        }
    }

    public void ReleaseTactile()
    {
        tactile = false;
    }

    public void StartTactile()
    {
        tactile = true;
    }

    private void SendActuatorValues(int motor1PWM, int motor2PWM, int motor3PWM, int motor4PWM, int solenoid1State, int solenoid2State, int servoPosition)
    {
        // Construct the message in the format "A:M1,M2,M3,M4,V1,V2,S"
        string message = $"A:{motor1PWM},{motor2PWM},{motor3PWM},{motor4PWM},{solenoid1State},{solenoid2State},{servoPosition}";
        Debug.Log($"Sending values: {message}");

        // Send the message to the Arduino
        serialController.SendSerialMessage(message);
    }
}
