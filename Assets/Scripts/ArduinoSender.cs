using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoSender : MonoBehaviour
{
    public SerialController serialController;

    // Update frequency in seconds
    public float updateInterval = 0.5f;
    private float timer = 0.0f;

    public int thumbIndexPWM;
    public int middlePWM;
    public int tactilePWM;
    bool M12;
    bool M4;

    bool release = false;
    float releaseTimer = 0f;

    public float pumpTime = 2.0f;
    float pumpTimer = 0f;

    public bool large = false;
    public bool hard = false;
    public bool pinch = false;
    public bool tactile = false;
    
    void Start()
    {
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        release = (Time.time < releaseTimer);

        M12 = (Time.time < pumpTimer);
        M4 = (Time.time < pumpTimer);

        // Periodically send values to the Arduino
        if (timer >= updateInterval)
        {
            SendActuatorValues((M12 ? thumbIndexPWM : 0), (tactile ? tactilePWM : 0), (M4 ? middlePWM : 0), (pinch ? 0 : 1), (release ? 1 : 0), (hard ? 0 : 180));
            timer = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseKin();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Pump();
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

    public void Pump()
    {
        pumpTimer = Time.time + pumpTime;
    }

    public void ReleaseKin()
    {
        releaseTimer = Time.time + updateInterval;
    }

    public void OpenGate()
    {
        hard = false;
    }

    public void CloseGate()
    {
        hard = true;
    }

    public void PinchOn()
    {
        pinch = true;
    }

    public void PinchOff()
    {
        pinch = false;
    }

    private void SendActuatorValues(int motors12PWM, int motor3PWM, int motor4PWM, int solenoid1State, int solenoid2State, int servoPosition)
    {
        // Construct the message in the format "A:M1,M2,M3,M4,V1,V2,S"
        string message = $"A:{motors12PWM},0,{motor3PWM},{motor4PWM},{solenoid1State},{solenoid2State},{servoPosition}";
        Debug.Log($"Sending values: {message}");

        // Send the message to the Arduino
        serialController.SendSerialMessage(message);
    }
}
