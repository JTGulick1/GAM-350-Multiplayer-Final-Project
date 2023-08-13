using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Voting : MonoBehaviour
{
    public int playernum;
    public Text thisTxt;
    public ExampleClient client;
    void Awake()
    {
        thisTxt = this.GetComponent<Text>();
    }
    public void CastVote()
    {
        if (client.castedvote == false)
        {
            thisTxt.color = Color.red;
            client.CastVote(playernum);
        }
    }
}
