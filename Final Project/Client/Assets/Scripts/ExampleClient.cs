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
    public GameObject loginScreen;

    //Day Info
    public GameObject GameScreen;
    public GameObject votingScreen;
    public GameObject willScreen;
    public GameObject ReadyBut;
    public GameObject voteBut;
    public GameObject willBut;
    public GameObject startTxt;
    public GameObject gameTxt;
    public GameObject Smurfs;
    public string Role;
    public Text roletext;
    public Text UICountdown;
    public InputField WillTXT;

    //Night Info
    public GameObject NightScreen;
    public GameObject waiting;
    public GameObject KillersPanel;
    public GameObject BodyguardsPanel;
    public GameObject HealersPanel;
    public GameObject ScardyCatsPanel;
    public GameObject SpysPanel;
    public GameObject InfiltratorPanel;

    //Win
    public GameObject winScreen;

    public int UserID;

    public float Timer;

    //Chat UI
    public InputField chatInput;
    public Text chat;

    private bool isTimerRunning;
    private bool firstday = false;
    public bool castedvote = false;

    //Camera Colors and Time Change Settings
    public Camera BG;
    public Color day = Color.cyan;
    public Color night = Color.black;
    public float changtime = 3.0f;

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

    // Update is called once per frame
    void Update()
    {
        if (Timer >= 0)
        {
            UICountdown.gameObject.SetActive(true);
            Timer -= Time.deltaTime;
            UICountdown.text = string.Format("Time Remaining : " + Timer);

        }
        if (Timer <= 0 && isTimerRunning == true && firstday == false)
        {
            UICountdown.gameObject.SetActive(false);
            isTimerRunning = false;
            gameTxt.SetActive(false);
            clientNet.CallRPC("TimeChange", UCNetwork.MessageReceiver.ServerOnly, -1, false);
            firstday = true;
        }
        if (Timer <= 0 && isTimerRunning == true && firstday == true)
        {
            UICountdown.gameObject.SetActive(false);
            isTimerRunning = false;
            gameTxt.SetActive(false);
            clientNet.CallRPC("ResetIts", UCNetwork.MessageReceiver.ServerOnly, -1);
            clientNet.CallRPC("VoteCount", UCNetwork.MessageReceiver.ServerOnly, -1);
        }
    }
    public void RecieveID(int ID)
    {
        UserID = ID;
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
        clientNet.CallRPC("GetPlayerID", UCNetwork.MessageReceiver.ServerOnly, -1);
        loginScreen.SetActive(false);
        GameScreen.SetActive(true);
        gameTxt.SetActive(false);
        UICountdown.gameObject.SetActive(false);
        voteBut.gameObject.SetActive(false);
        willBut.gameObject.SetActive(false);
        chatInput.gameObject.SetActive(false);
        chat.gameObject.SetActive(false);
        Debug.Log("OnNetStatusConnected called");
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

        // Tell the server we are ready
    }
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
    public void IsReady()
    {
        clientNet.CallRPC("PlayerIsReady", UCNetwork.MessageReceiver.ServerOnly, -1);
    }
    public void GameStart()
    {
        startTxt.SetActive(false);
        ReadyBut.SetActive(false);
        willBut.SetActive(false);
        gameTxt.SetActive(true);
    }
    public void RoleAssigment(int role)
    {
        if (role == 1)
        {
            Role = "HL";
            roletext.text = "Your Role is: Healer";
        }
        if (role == 2)
        {
            Role = "PI";
            roletext.text = "Your Role is: Private Investigator";
        }
        if (role == 3)
        {
            Role = "BG";
            roletext.text = "Your Role is: Bodyguard";
        }
        if (role == 4)
        {
            Role = "SC";
            roletext.text = "Your Role is: Scardy Cat";
        }
        if (role == 5)
        {
            Role = "NN";
            roletext.text = "Your Role is: Noisy Neighbor";
        }
        if (role == 6)
        {
            Role = "KI";
            roletext.text = "Your Role is: Killer";
        }
        if (role == 7)
        {
            Role = "SPY";
            roletext.text = "Your Role is: Spy";
        }
        if (role == 8)
        {
            Role = "IF";
            roletext.text = "Your Role is: Infiltrator";
        }
    }
    public void TimerCountdown(float timer)
    {
        Timer = timer;
        isTimerRunning = true;
    }
    public void Day()
    {
        waiting.gameObject.SetActive(false);
        voteBut.gameObject.SetActive(true);
        willBut.gameObject.SetActive(true);
        chatInput.gameObject.SetActive(true);
        chat.gameObject.SetActive(true);
        Smurfs.SetActive(true);
        castedvote = false;
        BG.backgroundColor = day;
    }
    public void Night()
    {
        voteBut.gameObject.SetActive(false);
        willBut.gameObject.SetActive(false);
        chatInput.gameObject.SetActive(false);
        chat.gameObject.SetActive(false);
        NightScreen.SetActive(true);
        waiting.SetActive(false);
        HealersPanel.SetActive(false);
        BodyguardsPanel.SetActive(false);
        KillersPanel.SetActive(false);
        SpysPanel.SetActive(false);
        InfiltratorPanel.SetActive(false);
        ScardyCatsPanel.SetActive(false);
        Smurfs.SetActive(false);

        BG.backgroundColor = night;
        if (Role == "HL")
        {
            HealersPanel.SetActive(true);
        }
        if (Role == "BG")
        {
            BodyguardsPanel.SetActive(true);
        }
        if (Role == "SC")
        {
            ScardyCatsPanel.SetActive(true);
        }
        if (Role == "KI")
        {
            KillersPanel.SetActive(true);
        }
        if (Role == "SPY")
        {
            SpysPanel.SetActive(true);
        }
        if (Role == "IF")
        {
            InfiltratorPanel.SetActive(true);
        }
        if (Role == "NN" || Role == "PI")
        {
            Waiting();
        }
    }
    public void VotingUI()
    {
        votingScreen.SetActive(true);
        GameScreen.SetActive(false);
    }
    public void LastWillUI()
    {
        willScreen.SetActive(true);
        GameScreen.SetActive(false);
    }
    public void ReturnToGame()
    {
        votingScreen.SetActive(false);
        willScreen.SetActive(false);
        GameScreen.SetActive(true);
    }
    public void CastVote(int playernum)
    {
        castedvote = true;
        clientNet.CallRPC("VoteCast", UCNetwork.MessageReceiver.ServerOnly, -1, playernum);
    }
    public void SendChat(string message)
    {
        message = chatInput.text;
        chatInput.text = "";
        clientNet.CallRPC("RecieveChat", UCNetwork.MessageReceiver.AllClients, -1, $"[{UserID}]: {message}");
    }
    public void RecieveChat(string message)
    {
        chat.text = chat.text + "\n" + message;
    }
    public void Waiting()
    {
        HealersPanel.SetActive(false);
        BodyguardsPanel.SetActive(false);
        KillersPanel.SetActive(false);
        SpysPanel.SetActive(false);
        InfiltratorPanel.SetActive(false);
        ScardyCatsPanel.SetActive(false);
        waiting.SetActive(true);
        clientNet.CallRPC("isWaiting", UCNetwork.MessageReceiver.ServerOnly, -1);
    }
    public void TryingToKill(int playernum)
    {
        if (Role == "KI")
        {
            clientNet.CallRPC("KillCast", UCNetwork.MessageReceiver.ServerOnly, -1, playernum);
        }
    }
    public void TryingToHeal(int playernum)
    {
        if (Role == "HL")
        {
            clientNet.CallRPC("HealCast", UCNetwork.MessageReceiver.ServerOnly, -1, playernum);
        }
    }
    public void Protect(int playernum)
    {
        if (Role == "BG")
        {
            clientNet.CallRPC("ProtectCast", UCNetwork.MessageReceiver.ServerOnly, -1, playernum);
        }
    }
    public void Distract(int playernum)
    {
        if (Role == "SC")
        {
            clientNet.CallRPC("DistractCast", UCNetwork.MessageReceiver.ServerOnly, -1, playernum);
        }
    }
    public void Spy(int playernum)
    {
        if (Role == "SPY")
        {
            clientNet.CallRPC("SpyCast", UCNetwork.MessageReceiver.ServerOnly, -1, playernum);
        }
    }
    public void Infultrate(int playernum)
    {
        if (Role == "IF")
        {
            clientNet.CallRPC("InfultrateCast", UCNetwork.MessageReceiver.ServerOnly, -1, playernum);
        }
    }
    public void UnalivePlayer()
    {
        if (Role == "KI")
        {
            clientNet.CallRPC("FoundKiller", UCNetwork.MessageReceiver.ServerOnly, -1);
        }
        else
        {
            clientNet.CallRPC("RecieveChat", UCNetwork.MessageReceiver.AllClients, -1, $"[{UserID}]: {WillTXT.text}");
            Application.Quit();
        }
    }
    public void GameCompleted()
    {
        GameScreen.SetActive(false);
        votingScreen.SetActive(false);
        willScreen.SetActive(false);
        NightScreen.SetActive(false);
        winScreen.SetActive(true);
    }
}