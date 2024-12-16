using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectMager : MonoBehaviour
{
    public GameObject objectMenu;
    public GameObject objectMenuButton;

    public GameObject table;
    public GameObject glassBottle;
    public GameObject poolNoodle;
    public GameObject flashlight;
    public GameObject waterBottle;
    public GameObject coin;

    public UnityEvent hardChosen;
    public UnityEvent softChosen;
    public UnityEvent largeChosen;
    public UnityEvent smallChosen;
    public UnityEvent pinchEnabled;
    public UnityEvent pinchDisabled;


    public Transform hand;


    public void SetGlassBottle()
    {
        table.SetActive(true);
        glassBottle.SetActive(true);
        poolNoodle.SetActive(false);
        flashlight.SetActive(false);
        waterBottle.SetActive(false);
        coin.SetActive(false);
        hardChosen.Invoke();
        largeChosen.Invoke();
        pinchDisabled.Invoke();
        hand.rotation = Quaternion.Euler(0f, 0f, -90f);
    }

    public void SetPoolNoodle()
    {
        table.SetActive(false);
        glassBottle.SetActive(false);
        poolNoodle.SetActive(true);
        flashlight.SetActive(false);
        waterBottle.SetActive(false);
        coin.SetActive(false);
        softChosen.Invoke();
        largeChosen.Invoke();
        pinchDisabled.Invoke();
        hand.rotation = Quaternion.Euler(0f, 0f, -90f);
    }

    public void SetFlashlight()
    {
        table.SetActive(true);
        glassBottle.SetActive(false);
        poolNoodle.SetActive(false);
        flashlight.SetActive(true);
        waterBottle.SetActive(false);
        coin.SetActive(false);
        hardChosen.Invoke();
        smallChosen.Invoke();
        pinchDisabled.Invoke();
        hand.rotation = Quaternion.Euler(-16.2f, 0f, -90f);
    }

    public void SetWaterBottle()
    {
        table.SetActive(true);
        glassBottle.SetActive(false);
        poolNoodle.SetActive(false);
        flashlight.SetActive(false);
        waterBottle.SetActive(true);
        coin.SetActive(false);
        softChosen.Invoke();
        smallChosen.Invoke();
        pinchDisabled.Invoke();
        hand.rotation = Quaternion.Euler(-16.2f, 0f, -90f);
    }

    public void SetCoin()
    {
        table.SetActive(true);
        glassBottle.SetActive(false);
        poolNoodle.SetActive(false);
        flashlight.SetActive(false);
        waterBottle.SetActive(false);
        coin.SetActive(true);
        hardChosen.Invoke();
        smallChosen.Invoke();
        pinchEnabled.Invoke();
        hand.rotation = Quaternion.Euler(0f, 0f, -90f);
    }

    public void SetNone()
    {
        table.SetActive(true);
        glassBottle.SetActive(false);
        poolNoodle.SetActive(false);
        flashlight.SetActive(false);
        waterBottle.SetActive(false);
        coin.SetActive(false);
        softChosen.Invoke();
        smallChosen.Invoke();
        pinchEnabled.Invoke();
        hand.rotation = Quaternion.Euler(0f, 0f, -90f);
    }

    public void EnterMenu()
    {
        objectMenuButton.SetActive(false);
        objectMenu.SetActive(true);
    }

    public void ExitMenu()
    {
        objectMenuButton.SetActive(true);
        objectMenu.SetActive(false);
    }
}
