using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExampleClient : MonoBehaviour
{
    public ClientNetwork clientNet;

    // Get the instance of the client
    static ExampleClient instance = null;

    // Are we in the process of logging into a server
    private bool loginInProcess = false;

    public GameObject loginScreen; //UI
    public GameObject gameScreen;
    public GameObject hit;
    public GameObject pass;
    public GameObject ready;

    //Player Texts
    public Text Player1; // UI
    public Text Player2;
    public Text Player3;
    public Text Player4;
    public Text Player5;
    public Text Player6;
    public Text Player7;
    public Text House;
    int housescore; // House Score
    int Points1; // points for players
    int Points2;
    int Points3;
    int Points4;
    int Points5;
    int Points6;
    int Points7;
    bool Done1 = false; //bools that tell the server which client is done
    bool Done2 = false;
    bool Done3 = false;
    bool Done4 = false;
    bool Done5 = false;
    bool Done6 = false;
    bool Done7 = false;
    int totalDone = 0; // total players done


    int currentPlayers = 7;

    // Singleton support
    public static ExampleClient GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("ExampleClient is uninitialized");
            return null;
        }
        return instance;
    }

    // Use this for initialization
    void Awake()
    {
        // Make sure we have a ClientNetwork to use
        if (clientNet == null)
        {
            clientNet = GetComponent<ClientNetwork>();
        }
        if (clientNet == null)
        {
            clientNet = (ClientNetwork)gameObject.AddComponent(typeof(ClientNetwork));
        }
    }

    // Start the process to login to a server
    public void ConnectToServer(string aServerAddress, int aPort)
    {
        if (loginInProcess)
        {
            return;
        }
        loginInProcess = true;

        ClientNetwork.port = aPort;
        clientNet.Connect(aServerAddress, ClientNetwork.port, "", "", "", 0);
    }
    public void RPCTest(int aInt)
    {
        Debug.Log("RPC Test has been called with " + aInt);
    }

    public void NewClientConnected(long aClientId, string aValue)
    {
        Debug.Log("RPC NewClientConnected has been called with " + aClientId + " " + aValue);
    }

    // Networking callbacks
    // These are all the callbacks from the ClientNetwork
    void OnNetStatusNone()
    {
        Debug.Log("OnNetStatusNone called");
    }
    void OnNetStatusInitiatedConnect()
    {
        Debug.Log("OnNetStatusInitiatedConnect called");
    }
    void OnNetStatusReceivedInitiation()
    {
        Debug.Log("OnNetStatusReceivedInitiation called");
    }
    void OnNetStatusRespondedAwaitingApproval()
    {
        Debug.Log("OnNetStatusRespondedAwaitingApproval called");
    }
    void OnNetStatusRespondedConnect()
    {
        Debug.Log("OnNetStatusRespondedConnect called");
    }
    void OnNetStatusConnected()
    {
        loginScreen.SetActive(false);
        Debug.Log("OnNetStatusConnected called");
        gameScreen.SetActive(true);
        clientNet.AddToArea(1);
    }

    void OnNetStatusDisconnecting()
    {
        Debug.Log("OnNetStatusDisconnecting called");

    }
    void OnNetStatusDisconnected()
    {
        Debug.Log("OnNetStatusDisconnected called");
        SceneManager.LoadScene("Client");

        loginInProcess = false;
    }
    public void OnChangeArea()
    {
        Debug.Log("OnChangeArea called");
    }

    // RPC Called by the server once it has finished sending all area initization data for a new area
    public void AreaInitialized()
    {
        Debug.Log("AreaInitialized called");
    }

    void OnDestroy()
    {
        if (clientNet.IsConnected())
        {
            clientNet.Disconnect("Peace out");
        }
    }

    public void Pass() // pass turn
    {
        clientNet.CallRPC("NextTurn", UCNetwork.MessageReceiver.ServerOnly, -1);
        hit.SetActive(false);
        pass.SetActive(false);
    }
    public void Hit() // hit function
    {
        clientNet.CallRPC("Hit", UCNetwork.MessageReceiver.ServerOnly, -1);
    }
    public void Ready() // is the player ready?
    {
        clientNet.CallRPC("PlayerIsReady", UCNetwork.MessageReceiver.ServerOnly, -1);
    }
    public void HouseSet(int num) // house set 1st time
    {
        housescore = num;
        House.text = "House Points = " + num;
        hit.SetActive(false);
        pass.SetActive(false);
        ready.SetActive(false);
        clientNet.CallRPC("PlayerCount", UCNetwork.MessageReceiver.ServerOnly, -1);
    }
    public void HouseReset(int num) // reset the house
    {
        housescore = num;
        House.text = "House Points = " + num;
    }
    public void StartTurn() // start this clients turn
    {
        hit.SetActive(true);
        pass.SetActive(true);
    }
    public void Addpoints(int playernum, int points)
    {
        if (playernum == 1)// if player 1 hits
        {
            if (Done1 == false) // if player 1 is done do not score him any points
            {
                Points1 += points; // hit points adds to total points
                StartRound(currentPlayers); // update score and text
            }
        }
        if (playernum == 2)// if player 2 hits
        {
            if (Done2 == false)
            {
                Points2 += points;
                StartRound(currentPlayers);
            }
        }
        if (playernum == 3)// if player 3 hits
        {
            if (Done3 == false)
            {
                Points3 += points;
                StartRound(currentPlayers);
            }
        }
        if (playernum == 4)// if player 4 hits
        {
            if (Done4 == false)
            {
                Points4 += points;
                StartRound(currentPlayers);
            }
        }
        if (playernum == 5)// if player 5 hits
        {
            if (Done4 == false)
            {
                Points4 += points;
                StartRound(currentPlayers);
            }
        }
        if (playernum == 6)// if player 6 hits
        {
            if (Done6 == false)
            {
                Points6 += points;
                StartRound(currentPlayers);
            }
        }
        if (playernum == 7) // if player 7 hits
        {
            if (Done7 == false)
            {
                Points6 += points;
                StartRound(currentPlayers);
            }
        }
    }
    public void StartRound(int num)
    {
        if (num >= 1)//Change Player 1
        {
            if (Points1 > housescore) // if they are higher then the house
            {
                if (Points1 > housescore && Points1 < 21) // this player won
                {
                    Player1.text = "Player 1's Points: " + Points1 + "   Player 1 Wins This Round"; // text for winning
                    Done1 = true; // Player 1 is done
                    totalDone++; // total players done +1
                }
                if (Points1 > housescore && Points1 > 21) // Did not win 
                {
                    Player1.text = "Player 1's Points: " + Points1 + "   Player 1 Broke 21"; // text for losing
                    Done1 = true;
                    totalDone++;
                }
            }
            if (Points1 < housescore) // if they are not higher then the house
            {
                Player1.text = "Player 1's Points: " + Points1; 
            }
        }
        if (num >= 2)//Change Player 2
        {
            if (Points2 > housescore)
            {
                if (Points2 > housescore && Points2 < 21)
                {
                    Player2.text = "Player 2's Points: " + Points2 + "   Player 2 Wins This Round";
                    Done2 = true;
                    totalDone++;
                }
                if (Points2 > housescore && Points2 > 21)
                {
                    Player2.text = "Player 2's Points: " + Points2 + "   Player 2 Broke 21";
                    Done2 = true;
                    totalDone++;
                }
            }
            if (Points2 < housescore)
            {
                Player2.text = "Player 2's Points: " + Points2;
            }
        }
        if (num >= 3)//Change Player 3
        {
            if (Points3 > housescore)
            {
                if (Points3 > housescore && Points3 < 21)
                {
                    Player3.text = "Player 3's Points: " + Points3 + "   Player 3 Wins This Round";
                    Done3 = true;
                    totalDone++;
                }
                if (Points3 > housescore && Points3 > 21)
                {
                    Player3.text = "Player 3's Points: " + Points3 + "   Player 3 Broke 21";
                    Done3 = true;
                    totalDone++;
                }
            }
            if (Points3 < housescore)
            {
                Player3.text = "Player 3's Points: " + Points3;
            }
        }
        if (num >= 4)//Change Player 4
        {
            if (Points4 > housescore)
            {
                if (Points4 > housescore && Points4 < 21)
                {
                    Player4.text = "Player 4's Points: " + Points4 + "   Player 4 Wins This Round";
                    Done4 = true;
                    totalDone++;
                }
                if (Points4 > housescore && Points4 > 21)
                {
                    Player4.text = "Player 4's Points: " + Points4 + "   Player 4 Broke 21";
                    Done4 = true;
                    totalDone++;
                }
            }
            if (Points4 < housescore)
            {
                Player4.text = "Player 4's Points: " + Points4;
            }
        }
        if (num >= 5)//Change Player 5
        {
            if (Points5 > housescore)
            {
                if (Points5 > housescore && Points5 < 21)
                {
                    Player5.text = "Player 5's Points: " + Points5 + "   Player 5 Wins This Round";
                    Done5 = true;
                    totalDone++;
                }
                if (Points5 > housescore && Points5 > 21)
                {
                    Player5.text = "Player 5's Points: " + Points5 + "   Player 5 Broke 21";
                    Done5 = true;
                    totalDone++;
                }
            }
            if (Points5 < housescore)
            {
                Player5.text = "Player 5's Points: " + Points5;
            }
        }
        if (num >= 6)//Change Player 6
        {
            if (Points6 > housescore)
            {
                if (Points6 > housescore && Points6 < 21)
                {
                    Player6.text = "Player 6's Points: " + Points6 + "   Player 6 Wins This Round";
                    Done6 = true;
                    totalDone++;
                }
                if (Points6 > housescore && Points6 > 21)
                {
                    Player6.text = "Player 6's Points: " + Points6 + "   Player 6 Broke 21";
                    Done6 = true;
                    totalDone++;
                }
            }
            if (Points6 < housescore)
            {
                Player6.text = "Player 6's Points: " + Points6;
            }
        }
        if (num >= 7) //Change Player 7
        {
            if (Points7 > housescore)
            {
                if (Points7 > housescore && Points7 < 21)
                {
                    Player7.text = "Player 7's Points: " + Points7 + "   Player 7 Wins This Round";
                    Done7 = true;
                    totalDone++;
                }
                if (Points7 > housescore && Points7 > 21)
                {
                    Player7.text = "Player 7's Points: " + Points7 + "   Player 7 Broke 21";
                    Done7 = true;
                    totalDone++;
                }
            }
            if (Points7 < housescore)
            {
                Player7.text = "Player 7's Points: " + Points7;
            }
        }
        if (currentPlayers == totalDone) // are all players done?
        {
            clientNet.CallRPC("Reset", UCNetwork.MessageReceiver.ServerOnly, -1);
        }
    }
    public void GetPlayerCount(int count) // how many players are currently in the game
    {
        currentPlayers = count;
    }
    public void RestartGame() // Reset Game when everyone can not play
    {
        totalDone = 0;
        Points1 = 0;
        Points2 = 0;
        Points3 = 0;
        Points4 = 0;
        Points5 = 0;
        Points6 = 0;
        Points7 = 0;
        Done1 = false;
        Done2 = false;
        Done3 = false;
        Done4 = false;
        Done5 = false;
        Done6 = false;
        Done7 = false;
        clientNet.CallRPC("HouseReset", UCNetwork.MessageReceiver.ServerOnly, -1);
    }
}


