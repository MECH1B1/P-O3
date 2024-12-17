using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerHandler : MonoBehaviour
{
    bool thumbTouching = false;
    bool indexTouching = false;
    bool middleTouching = false;
    bool previouslyTouching = false;

    public ArduinoSender arduinoSender;

    public Renderer handRenderer;
    public Material white;
    public Material green;

    public UnityEvent startTactile;
    public UnityEvent releaseTactile;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.gameObject.name} has entered {gameObject.name}.");

        if (other.CompareTag("Thumb"))
        {
            thumbTouching = true;
        }
        else if (other.CompareTag("Index"))
        {
            indexTouching = true;
        }
        else if (other.CompareTag("Middle"))
        {
            middleTouching = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Thumb"))
        {
            thumbTouching = false;
        }
        else if (other.CompareTag("Index"))
        {
            indexTouching = false;
        }
        else if (other.CompareTag("Middle"))
        {
            middleTouching = false;
        }
    }

    void Update()
    {
        bool allTouching = thumbTouching && indexTouching && (middleTouching || arduinoSender.pinch);

        Debug.Log($"Thumb: {thumbTouching}, Ind: {indexTouching}, Mid: {middleTouching}");

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
