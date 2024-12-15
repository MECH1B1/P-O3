using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    public Text recordingText;
    public Text sensorText;

    public HandModelRotationRecorder dataWriter;
    public FlexDataReader flexReader;

    // Update is called once per frame
    void Update()
    {
        recordingText.text = $"{(dataWriter.recording ? "[OPNEMEN...]" : "[GEPAUZEERD]")} \nDatapunten: {dataWriter.dataPoints}";
        sensorText.text = $"Sensoren: \n{flexReader.sensorValues["Sensor 1"]}\n{flexReader.sensorValues["Sensor 2"]}\n{flexReader.sensorValues["Sensor 3"]}\n{flexReader.sensorValues["Sensor 4"]}\n{flexReader.sensorValues["Sensor 5"]}\n{flexReader.sensorValues["Sensor Hall"]}";
    }
}
