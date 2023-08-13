using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healing : MonoBehaviour
{
    public int playernum;
    public Text thisTxt;
    public ExampleClient client;
    void Awake()
    {
        thisTxt = this.GetComponent<Text>();
    }
    public void Heal()
    {
        thisTxt.color = Color.red;
        client.TryingToHeal(playernum);
        client.Waiting();
    }
}
