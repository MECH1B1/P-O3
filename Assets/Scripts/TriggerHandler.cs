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

    bool hard;
    bool large;

    public UnityEvent startTactile;
    public UnityEvent releaseTactile;

    public bool test1 = false;
    public bool test2 = false;
    public bool test3 = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "R_ThumbTip")
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

    void Awake()
    {
        hard = gameObject.tag.Split(';')[0] == "Hard";
        large = gameObject.tag.Split(';')[1] == "Large";
    }

    void Update()
    {
        bool allTouching = thumbTouching && indexTouching && middleTouching;
        test1 = thumbTouching;
        test2 = indexTouching;
        test3 = middleTouching;

        if (allTouching && !previouslyTouching)
        {
            startTactile.Invoke();
        }

        if (!allTouching && previouslyTouching)
        {
            startTactile.Invoke();
        }

        previouslyTouching = allTouching;
    }
}
