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

    public UnityEvent hardChosen;
    public UnityEvent softChosen;


    public void SetGlassBottle()
    {
        table.SetActive(true);
        glassBottle.SetActive(true);
        poolNoodle.SetActive(false);
        hardChosen.Invoke();
    }

    public void SetPoolNoodle()
    {
        table.SetActive(false);
        glassBottle.SetActive(false);
        poolNoodle.SetActive(true);
        softChosen.Invoke();
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
