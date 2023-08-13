using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Infiltrating : MonoBehaviour
{
    public int playernum;
    public Text thisTxt;
    public ExampleClient client;
    void Awake()
    {
        thisTxt = this.GetComponent<Text>();
    }
    public void Infultrate()
    {
        thisTxt.color = Color.red;
        client.Infultrate(playernum);
        client.Waiting();
    }
}
