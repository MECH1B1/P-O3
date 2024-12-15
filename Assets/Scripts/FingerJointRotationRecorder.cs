using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class HandModelRotationRecorder : MonoBehaviour
{
    [Header("Animator and Parameters")]
    public Animator handAnimator; // Reference to the Animator component

    [Header("Animator Parameter Names")]
    public string pinchParam = "PinchBlend";
    public string thumbParam1 = "ThumbBlend1";
    public string thumbParam2 = "ThumbBlend2";
    public string indexParam1 = "IndexBlend1";
    public string indexParam2 = "IndexBlend2";
    public string middleRingLittleParam1 = "MiddleRingLittleBlend1";
    public string middleRingLittleParam2 = "MiddleRingLittleBlend2";

    [Header("Sensor Data Source")]
    public FlexDataReader flexDataReader; // Reference to the FlexDataReader script

    private float snapshotInterval = 0.1f; // Time interval for snapshots
    private float lastSnapshotTime = 0f;   // Last snapshot time
    private bool headerWritten = false;    // Flag to check if the header has been written

    private string filePath = "HandAnimatorParametersWithSensors.csv"; // Path to save the CSV file
    private StreamWriter writer;

    public int dataPoints = 0;

    public bool recording = false;

    void Start()
    {
        if (handAnimator == null)
        {
            Debug.LogError("Animator is not assigned.");
            return;
        }

        if (flexDataReader == null)
        {
            Debug.LogError("FlexDataReader is not assigned.");
            return;
        }

        // Initialize the StreamWriter for writing parameter data
        writer = new StreamWriter(filePath, false); // 'false' to overwrite the file if it exists
    }

    void Update()
    {
        // Write the header if not already done
        if (!headerWritten)
        {
            WriteHeader();
            headerWritten = true;
            dataPoints = 0;
        }

        // Capture parameter data at intervals after the header is written
        if (recording && headerWritten && Time.time >= lastSnapshotTime + snapshotInterval)
        {
            CaptureAnimatorParameters();
            lastSnapshotTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.R) && !recording)
        {
            Debug.Log("Started recording.");
            recording = true;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Recording stopped.");
            recording = false;
        }
    }

    private void WriteHeader()
    {
        // Start the header with parameter fields
        string header = "Time,PinchBlend,ThumbBlend1,ThumbBlend2,IndexBlend1,IndexBlend2,MiddleRingLittleBlend1,MiddleRingLittleBlend2";

        // Add the keys of the sensorValues dictionary to the header
        foreach (var key in flexDataReader.sensorValues.Keys)
        {
            header += $",{key}";
        }

        // Write the header line to the CSV file
        writer.WriteLine(header);
        Debug.Log("Header written to CSV file.");
    }

    private void CaptureAnimatorParameters()
    {
        // Retrieve animator parameters
        float pinchValue = handAnimator.GetFloat(pinchParam);
        float thumb1Value = handAnimator.GetFloat(thumbParam1);
        float thumb2Value = handAnimator.GetFloat(thumbParam2);
        float index1Value = handAnimator.GetFloat(indexParam1);
        float index2Value = handAnimator.GetFloat(indexParam2);
        float middleRingLittle1Value = handAnimator.GetFloat(middleRingLittleParam1);
        float middleRingLittle2Value = handAnimator.GetFloat(middleRingLittleParam2);

        // Start the line with parameter data, using InvariantCulture for consistent decimal formatting
        string line = string.Format(CultureInfo.InvariantCulture,
            "{0:F4},{1:F4},{2:F4},{3:F4},{4:F4},{5:F4},{6:F4},{7:F4}",
            Time.time,             // Current timestamp
            pinchValue,            // Pinch parameter
            thumb1Value, thumb2Value,  // Thumb parameters
            index1Value, index2Value,  // Index finger parameters
            middleRingLittle1Value, middleRingLittle2Value); // Middle, ring, and little fingers

        // Add the sensor values from the dictionary
        foreach (var key in flexDataReader.sensorValues.Keys)
        {
            line += string.Format(CultureInfo.InvariantCulture, ",{0:F4}", flexDataReader.sensorValues[key]);
        }

        // Write the formatted line to the CSV file
        writer.WriteLine(line);

        // Ensure data is written to the file immediately
        writer.Flush();

        dataPoints += 1;
    }

    void OnDestroy()
    {
        // Close the StreamWriter when the script is destroyed
        if (writer != null)
        {
            writer.Close();
        }
    }
}
