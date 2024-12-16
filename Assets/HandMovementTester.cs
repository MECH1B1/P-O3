using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMovementTester : MonoBehaviour
{
    // Reference to the Animator component on the hand model
    public Animator handAnimator;

    // Define the blend tree parameters for each finger group
    [Header("Blend Tree Parameter Names")]
    private string pinchParam1 = "Pinch";
    private string thumbParam1 = "Thumb_param1";
    private string thumbParam2 = "Thumb_param2";
    private string indexParam1 = "Index_param1";
    private string indexParam2 = "Index_param2";
    private string middleRingLittleParam1 = "MiddleRingLittle_param1";
    private string middleRingLittleParam2 = "MiddleRingLittle_param2";

    public float pinchValue;
    public float thumb1Value, thumb2Value;
    public float index1Value, index2Value;
    public float middleRingLittle1Value, middleRingLittle2Value;

    void Update()
    {
        handAnimator.SetFloat(pinchParam1, pinchValue);
        handAnimator.SetFloat(thumbParam1, thumb1Value);
        handAnimator.SetFloat(thumbParam2, thumb2Value);
        handAnimator.SetFloat(indexParam1, index1Value);
        handAnimator.SetFloat(indexParam2, index2Value);
        handAnimator.SetFloat(middleRingLittleParam1, middleRingLittle1Value);
        handAnimator.SetFloat(middleRingLittleParam2, middleRingLittle2Value);
    }
}
