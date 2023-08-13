using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Square : MonoBehaviour
{
    //Tactics Info
    public GameObject TacticsClient;
    public TacticsClient tactics;

    public int hor, ver; // Position

    public bool isplayer; // is it a player
    public GameObject[] delete;
    int numOfDeletes;

    void Awake()
    {
        TacticsClient = GameObject.FindGameObjectWithTag("TacticsClient"); // Get Tactics Client
        tactics = TacticsClient.GetComponent<TacticsClient>();
        ver = tactics.currHeight; //Get Height
        hor = tactics.currWidth; // Get Width
        if (this.gameObject.tag == "Player")
        {
            isplayer = true; // is this a Player?
        }
    }
    void OnMouseDown()
    {
        tactics.RequestMove(ver, hor); //Move Here
    }
}