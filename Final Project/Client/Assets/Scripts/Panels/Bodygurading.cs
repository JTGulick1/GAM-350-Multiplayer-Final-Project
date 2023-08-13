using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bodygurading : MonoBehaviour
{
    public int playernum;
    public Text thisTxt;
    public ExampleClient client;
    void Awake()
    {
        thisTxt = this.GetComponent<Text>();
    }
    public void Protect()
    {
        thisTxt.color = Color.red;
        client.Protect(playernum);
        client.Waiting();
    }
}
