using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HandAnimationController : MonoBehaviour
{
    // Reference to the Animator component on the hand model
    public Animator handAnimator;

    // Define the blend tree parameters for each finger group
    [Header("Blend Tree Parameter Names")]
    public string pinchParam1 = "Pinch";
    public string thumbParam1 = "Thumb_param1";
    public string thumbParam2 = "Thumb_param2";
    public string indexParam1 = "Index_param1";
    public string indexParam2 = "Index_param2";
    public string middleRingLittleParam1 = "MiddleRingLittle_param1";
    public string middleRingLittleParam2 = "MiddleRingLittle_param1";

    public bool loopSequence = false;

    [SerializeField] private AudioClip poseSound;
    private AudioSource audioSource;
    public float audioDelay = 0f;
    public bool playTransitionSound;
    private bool playSound = false;

    [Header("Bend Coefficient")]
    public float bendCoefficient = 1f; // Multiplier for target values

    // Define the sequence data
    [System.Serializable]
    public class FingerPoseSequence
    {
        public float pinchTarget;  // Target value for pinch parameter
        public float thumb1Target; // Target value for ThumbBlend1
        public float thumb2Target; // Target value for ThumbBlend2
        public float index1Target; // Target value for IndexBlend1
        public float index2Target; // Target value for IndexBlend2
        public float middleRingLittle1Target; // Target value for MiddleRingLittleBlend1
        public float middleRingLittle2Target; // Target value for MiddleRingLittleBlend2
        public float duration;     // How long to reach this pose
    }

    [Header("Pose Sequences")]
    public List<FingerPoseSequence> poseSequences;

    public bool useFile = false; // Whether to load poseSequences from file

    public string fileName = "poseSequences.json"; // Name of the file to load/save
    private string lastFileName;

    private bool zeroPosition = false;
    FingerPoseSequence zeroPose = new FingerPoseSequence
    {
        pinchTarget = 0f,
        thumb1Target = 0f,
        thumb2Target = 0f,
        index1Target = 0f,
        index2Target = 0f,
        middleRingLittle1Target = 0f,
        middleRingLittle2Target = 0f,
        duration = 1f // Optional: Duration can be set to any value, e.g., 1f if needed
    };

    private int currentPoseIndex = 0; // Tracks the current pose in the sequence
    private float transitionTimer = 0f; // Timer to manage pose transitions
    private bool isTransitioning = false; // Flag to manage state

    // Current blend values for all parameters
    private float pinchValue;
    private float thumb1Value, thumb2Value;
    private float index1Value, index2Value;
    private float middleRingLittle1Value, middleRingLittle2Value;

    // Blend values for all parameters on the start of a transition
    private float pinch_start;
    private float thumb1_start, thumb2_start;
    private float index1_start, index2_start;
    private float middleRingLittle1_start, middleRingLittle2_start;

    public float timerTest;

    void Start()
    {
        // Initialize blend values to current Animator parameters
        pinchValue = handAnimator.GetFloat(pinchParam1);
        thumb1Value = handAnimator.GetFloat(thumbParam1);
        thumb2Value = handAnimator.GetFloat(thumbParam2);
        index1Value = handAnimator.GetFloat(indexParam1);
        index2Value = handAnimator.GetFloat(indexParam2);
        middleRingLittle1Value = handAnimator.GetFloat(middleRingLittleParam1);
        middleRingLittle2Value = handAnimator.GetFloat(middleRingLittleParam2);

        if (poseSequences.Count > 0)
            StartSequence();

        audioSource = GetComponent<AudioSource>();
    }

    public void StartSequence()
    {
        currentPoseIndex = 0;
        transitionTimer = 0f;
        isTransitioning = true;

        pinch_start = pinchValue;
        thumb1_start = thumb1Value;
        thumb2_start = thumb2Value;
        index1_start = index1Value;
        index2_start = index2Value;
        middleRingLittle1_start = middleRingLittle1Value;
        middleRingLittle2_start = middleRingLittle2Value;
    }

    void Update()
    {
        // Load pose sequences from file if useFile is enabled
        if (useFile && lastFileName != fileName)
        {
            LoadPoseSequencesFromFile();
            lastFileName = fileName;
        }

        // Check for "S" key press to save pose sequences
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePoseSequencesToFile();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            zeroPosition = !zeroPosition;
            Debug.Log(zeroPosition ? "Set position to zero" : "Canceled zero position");
            lastFileName = "";
        }

        if (zeroPosition)
        {
            poseSequences = new List<FingerPoseSequence> { zeroPose };
            currentPoseIndex = 0;
        }

        if (isTransitioning && poseSequences.Count > 0)
        {
            FingerPoseSequence currentPose = poseSequences[currentPoseIndex];

            if (transitionTimer >= (currentPose.duration + audioDelay) % currentPose.duration && playSound)
            {
                if (currentPoseIndex == 0)
                    audioSource.pitch = 1.5f;
                else
                    audioSource.pitch = 1.0f;

                audioSource.Play();
                playSound = false;
            }

            // Increment the transition timer
            transitionTimer += Time.deltaTime;

            // Lerp the values for a smooth transition, applying the bendCoefficient
            pinchValue = Mathf.Lerp(pinch_start, currentPose.pinchTarget * bendCoefficient, transitionTimer / currentPose.duration);
            thumb1Value = Mathf.Lerp(thumb1_start, currentPose.thumb1Target * bendCoefficient, transitionTimer / currentPose.duration);
            thumb2Value = Mathf.Lerp(thumb2_start, currentPose.thumb2Target * bendCoefficient, transitionTimer / currentPose.duration);
            index1Value = Mathf.Lerp(index1_start, currentPose.index1Target * bendCoefficient, transitionTimer / currentPose.duration);
            index2Value = Mathf.Lerp(index2_start, currentPose.index2Target * bendCoefficient, transitionTimer / currentPose.duration);
            middleRingLittle1Value = Mathf.Lerp(middleRingLittle1_start, currentPose.middleRingLittle1Target * bendCoefficient, transitionTimer / currentPose.duration);
            middleRingLittle2Value = Mathf.Lerp(middleRingLittle2_start, currentPose.middleRingLittle2Target * bendCoefficient, transitionTimer / currentPose.duration);

            // Set Animator parameters
            handAnimator.SetFloat(pinchParam1, pinchValue);
            handAnimator.SetFloat(thumbParam1, thumb1Value);
            handAnimator.SetFloat(thumbParam2, thumb2Value);
            handAnimator.SetFloat(indexParam1, index1Value);
            handAnimator.SetFloat(indexParam2, index2Value);
            handAnimator.SetFloat(middleRingLittleParam1, middleRingLittle1Value);
            handAnimator.SetFloat(middleRingLittleParam2, middleRingLittle2Value);

            // Check if transition is complete
            if (transitionTimer >= currentPose.duration)
            {
                transitionTimer = 0f;
                currentPoseIndex++;

                pinch_start = pinchValue;
                thumb1_start = thumb1Value;
                thumb2_start = thumb2Value;
                index1_start = index1Value;
                index2_start = index2Value;
                middleRingLittle1_start = middleRingLittle1Value;
                middleRingLittle2_start = middleRingLittle2Value;

                if (playTransitionSound)
                    playSound = true;

                if (currentPoseIndex >= poseSequences.Count)
                {
                    if (loopSequence)
                    {
                        currentPoseIndex = 0; // Loop Sequence
                    }
                    else
                    {
                        isTransitioning = false; // Sequence is complete
                    }
                }
            }
        }
    }

    // Save the pose sequences to a file
    private void SavePoseSequencesToFile()
    {
        string path = Path.Combine(Application.dataPath, fileName);
        string json = JsonUtility.ToJson(new PoseSequenceListWrapper(poseSequences), true);
        File.WriteAllText(path, json);
        Debug.Log("Pose sequences saved to: " + path);
    }

    // Load the pose sequences from a file
    private void LoadPoseSequencesFromFile()
    {
        // Path to the 'Pose Sequences' folder in the Assets directory
        string folderPath = Path.Combine(Application.dataPath, "Pose Sequences");
        string path = Path.Combine(folderPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PoseSequenceListWrapper wrapper = JsonUtility.FromJson<PoseSequenceListWrapper>(json);
            poseSequences = wrapper.poseSequences;
            Debug.Log("Pose sequences loaded from: " + path);
        }
        else
        {
            Debug.LogWarning("Pose sequences file not found at: " + path);
        }
    }

    // Wrapper class for JSON serialization of List<FingerPoseSequence>
    [System.Serializable]
    private class PoseSequenceListWrapper
    {
        public List<FingerPoseSequence> poseSequences;

        public PoseSequenceListWrapper(List<FingerPoseSequence> poseSequences)
        {
            this.poseSequences = poseSequences;
        }
    }
}
