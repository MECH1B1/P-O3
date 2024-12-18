using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public class HandController : MonoBehaviour
{
    [Header("Barracuda Model")]
    public NNModel onnxModel;
    private IWorker worker;

    [Header("Hand Animator")]
    public Animator handAnimator; // Reference to the hand Animator component

    [Header("Flex Sensor Data")]
    public FlexDataReader flexDataReader;

    [Header("Scaling Parameters")]
    public TextAsset inputScalerData;
    public TextAsset outputScalerData;

    private float[] inputMean;
    private float[] inputStd;
    private float[] outputMean;
    private float[] outputStd;

    public float bendMultiplier = 1f;

    // List of animator parameter names corresponding to predictions
    private List<string> animatorParameters = new List<string>
    {
        "Pinch",
        "Thumb_param1",
        "Thumb_param2",
        "Index_param1",
        "Index_param2",
        "MiddleRingLittle_param1",
        "MiddleRingLittle_param2"
    };

    void Start()
    {
        if (onnxModel == null)
        {
            Debug.LogError("ONNX model is not assigned.");
            return;
        }

        try
        {
            var loadedModel = ModelLoader.Load(onnxModel);
            worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, loadedModel);
            Debug.Log("Worker successfully initialized.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing Barracuda worker: {e.Message}");
        }

        if (handAnimator == null)
        {
            Debug.LogError("Hand Animator is not assigned.");
            return;
        }

        if (flexDataReader == null)
        {
            Debug.LogError("FlexDataReader is not assigned.");
            return;
        }

        // Load scaling parameters
        if (inputScalerData != null)
        {
            var inputScaler = JsonUtility.FromJson<ScalerData>(inputScalerData.text);
            inputMean = inputScaler.mean;
            inputStd = inputScaler.std;
        }
        else
        {
            Debug.LogError("Input scaler data is not assigned.");
        }

        if (outputScalerData != null)
        {
            var outputScaler = JsonUtility.FromJson<ScalerData>(outputScalerData.text);
            outputMean = outputScaler.mean;
            outputStd = outputScaler.std;
        }
        else
        {
            Debug.LogError("Output scaler data is not assigned.");
        }
    }

    void Update()
    {
        if (flexDataReader.sensorValues.Count == 0 || inputMean == null || inputStd == null || outputMean == null || outputStd == null)
            return;

        // Prepare input tensor
        float[] sensorInputs = new float[flexDataReader.sensorValues.Count];
        int i = 0;
        foreach (var value in flexDataReader.sensorValues.Values)
        {
            sensorInputs[i] = (value - inputMean[i]) / inputStd[i];
            i++;
        }

        // Create a tensor from the scaled sensor data
        Tensor inputTensor = new Tensor(1, sensorInputs.Length, sensorInputs);

        // Run the model
        Tensor outputTensor = worker.Execute(inputTensor).PeekOutput();

        // Extract predictions
        float[] predictions = outputTensor.ToReadOnlyArray();

        // Unscale predictions
        for (int j = 0; j < predictions.Length; j++)
        {
            predictions[j] = (predictions[j] * outputStd[j] + outputMean[j]) * bendMultiplier;
        }

        // Apply predictions to animator parameters
        for (int paramIndex = 0; paramIndex < animatorParameters.Count; paramIndex++)
        {
            if (paramIndex < predictions.Length)
            {
                handAnimator.SetFloat(animatorParameters[paramIndex], predictions[paramIndex]);
            }
        }

        // Dispose of the tensors to free memory
        inputTensor.Dispose();
        outputTensor.Dispose();
    }

    private void OnDestroy()
    {
        worker?.Dispose();
    }

    [Serializable]
    private class ScalerData
    {
        public float[] mean;
        public float[] std;
    }
}
