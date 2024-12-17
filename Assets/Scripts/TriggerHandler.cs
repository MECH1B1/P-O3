using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerHandler : MonoBehaviour
{
    bool previouslyTouching = false;

    public ArduinoSender arduinoSender;

    public Renderer handRenderer;
    public Material white;
    public Material green;
    public float handTouchDelay = 0.01f;

    float thumbTime = 0f;
    float indexTime = 0f;
    float middleTime = 0f;

    public UnityEvent startTactile;
    public UnityEvent releaseTactile;

    void OnTriggerStay(Collider other)
    {
        // Debug.Log($"{other.gameObject.name} has entered {gameObject.name}.");

        if (other.CompareTag("Thumb"))
        {
            thumbTime = Time.time + handTouchDelay;
        }
        else if (other.CompareTag("Index"))
        {
            indexTime = Time.time + handTouchDelay;
        }
        else if (other.CompareTag("Middle"))
        {
            middleTime = Time.time + handTouchDelay;
        }
    }

    void Update()
    {
        bool thumbTouching = Time.time < thumbTime;
        bool indexTouching = Time.time < indexTime;
        bool middleTouching = Time.time < middleTime;

        bool allTouching = thumbTouching && indexTouching && (middleTouching || arduinoSender.pinch);

        // Debug.Log($"Thumb: {thumbTouching}, Ind: {indexTouching}, Mid: {middleTouching}");

        if (allTouching && !previouslyTouching)
        {
            startTactile.Invoke();
            handRenderer.material = green;
        }

        if (!allTouching && previouslyTouching)
        {
            releaseTactile.Invoke();
            handRenderer.material = white;
        }

        previouslyTouching = allTouching;
    }
}
