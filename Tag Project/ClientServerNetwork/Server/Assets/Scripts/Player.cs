using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ExampleServer exampleServer;
    public int ID;
    public bool isIt = false;
    void Start()
    {
        exampleServer = GameObject.Find("ExampleServer").GetComponent<ExampleServer>(); // get the ExampleServer SC
        ID = exampleServer.GetID(); // Give Player ID number
        exampleServer.ID += 501; // Add 501 so id number follows properly
    }
}
