using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Killing : MonoBehaviour
{
    public int playernum;
    public Text thisTxt;
    public ExampleClient client;
    void Awake()
    {
        thisTxt = this.GetComponent<Text>();
    }
    public void Kill()
    {
        thisTxt.color = Color.red;
        client.TryingToKill(playernum);
        client.Waiting();
    }
}