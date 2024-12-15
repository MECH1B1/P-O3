/*
using UnityEngine;
using System.Collections;
using System.Globalization;

public class FlexDataReader : MonoBehaviour
{
    public SerialController serialController;
    int analogValue = 0;
    float voltage = 0.0f;
    public float flexResistance10k = 0.0f;
    
    public float updateRate;
    private float updateTimer = 0;

    public bool logData = false;

    // Initialization
    void Start()
    {
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
    }

    // Executed each frame
    void Update()
    {
        if (Time.time >= updateTimer + updateRate)
        {
            UpdateValues();
            updateTimer = Time.time;
        }
        if(logData)
        {
            LogValues();
        }
    }

    private void UpdateValues()
    {
        string message = serialController.ReadSerialMessage();
        string[] parts;

        if (message == null)
            return;


        int sensor = message[0];
        parts = message.Substring(4).Split(';');



        // Check if the message is plain data or a connect/disconnect event.
        if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
            Debug.Log("Connection established");
        else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
            Debug.Log("Connection attempt failed or disconnection detected");
        else
            foreach (string part in parts)
            {
                string[] keyValue = part.Split(':');
                // Debug.Log($"Key: [{keyValue[0]}], Value: [{keyValue[1]}]");

                if (keyValue.Length == 2)
                {
                    switch (keyValue[0].Trim())
                    {
                        case "Analog Value":
                            int.TryParse(keyValue[1].Trim(), out analogValue);
                            break;
                        case "Voltage":
                            float.TryParse(keyValue[1], NumberStyles.Float, CultureInfo.InvariantCulture, out voltage);
                            break;
                        case "Flex Sensor Resistance":
                            bool succes = float.TryParse(keyValue[1], NumberStyles.Float, CultureInfo.InvariantCulture, out flexResistance10k);
                            break;
                    }
                }
            }
            
            flexResistance = (resistorValue * (5.0 - voltage)) / voltage;
    }

    private void LogValues()
    {
        Debug.Log($"Sensor {sensor} --- Analog Value: {analogValue}, Voltage: {voltage}V, Flex Sensor Resistance: {flexResistance} ohms");
    }
}
*/
