using UnityEngine;
using System.Collections.Generic;

public class FlexDataReader : MonoBehaviour
{
    public SerialController serialController;

    public bool logData = false;

    // Dictionary to store the values for each sensor
    [HideInInspector] public Dictionary<string, int> sensorValues = new Dictionary<string, int>();
    private List<string> sensorKeys = new List<string> { "Sensor 1", "Sensor 2", "Sensor 3", "Sensor 4", "Sensor 5", "Sensor Hall" };

    // Initialization
    void Start()
    {
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();

        // Initialize the dictionary with default values
        foreach (var key in sensorKeys)
        {
            sensorValues[key] = 0;
        }
    }

    // Handle incoming messages from the Arduino
    public void OnMessageArrived(string message)
    {
        if (!string.IsNullOrEmpty(message) && message.StartsWith("U:"))
        {
            UpdateValues(message);
        }

        if (logData)
        {
            LogValues();
        }
    }

    public void OnConnectionEvent(bool isConnected)
    {
        Debug.Log(isConnected ? "Device connected" : "Device disconnected");
    }

    private void UpdateValues(string message)
    {
        try
        {
            // Remove the "U:" prefix and split the message into sensor values
            string[] values = message.Substring(2).Split(',');

            // Ensure the message contains exactly six values
            if (values.Length != sensorKeys.Count)
            {
                Debug.LogWarning($"Invalid message format: {message}");
                return;
            }

            // Update the dictionary with the parsed values
            for (int i = 0; i < sensorKeys.Count; i++)
            {
                if (int.TryParse(values[i], out int parsedValue))
                {
                    sensorValues[sensorKeys[i]] = parsedValue;
                }
                else
                {
                    Debug.LogWarning($"Failed to parse sensor value: {values[i]}");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error processing message: {message}. Exception: {ex.Message}");
        }
    }

    private void LogValues()
    {
        foreach (KeyValuePair<string, int> entry in sensorValues)
        {
            // Log the key (sensor name) and its corresponding value
            Debug.Log($"{entry.Key}: {entry.Value}");
        }
    }
}
