using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TacticsClient : MonoBehaviour
{
    public ClientNetwork clientNet;
    public Square square;

    // Are we in the process of logging into a server
    private bool loginInProcess = false;

    //Cameras
    public GameObject menuCam;
    public GameObject gameCam;


    //UI
    public GameObject loginScreen;
    public GameObject readyScreen;
    public GameObject playScreen;
    public InputField playname;
    public Text gameStartTimer;
    public Text chatBox;
    public Text health;
    public Text playerTurn;
    public InputField chat;
    public Dropdown currClassUI;


    //Class Objects
    public GameObject warrior;
    public GameObject rogue;
    public GameObject wizzard;


    //Board
    public GameObject movable;
    public GameObject blocked;
    int boardId;

    // Represents a player
    class Player
    {
        public string name = "";
        public int playerId = 0;
        public int characterClass = 0;
        public int health = 0;
        public int team = 0;
        public int xPos;
        public int yPos;
        public GameObject playerObject = null;
        public bool isready = false;
        public bool isTurn = false;
    }
    Dictionary<int, Player> players;

    public GameObject[] board;

    // My player id
    int playerId;
    int type;
    string currClass;

    //Map Squares
    public int mapWidth;
    public int mapHeight;
    public int currHeight;
    public int currWidth;

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
        Debug.Log("Logging In To Server");
        if (loginInProcess)
        {
            return;
        }
        loginInProcess = true;

        ClientNetwork.port = aPort;
        clientNet.Connect(aServerAddress, ClientNetwork.port, playname.text, "", currClass, 0);
    }

    void OnNetStatusConnected()
    {
        //Cam and UI changes
        loginScreen.SetActive(false);
        menuCam.SetActive(false);
        readyScreen.SetActive(true);
        gameCam.SetActive(true);
        Debug.Log("OnNetStatusConnected called");
        name = playname.text; // Set Name
        SetName(name);
        type = currClassUI.value + 1; // Set Class
        SetCharacterType(type);
    }

    void OnNetStatusDisconnected()
    {
        Debug.Log("OnNetStatusDisconnected called");
        SceneManager.LoadScene("Client");

        loginInProcess = false;
    }

    void OnDestroy()
    {
        if (clientNet.IsConnected())
        {
            clientNet.Disconnect("Peace out");
        }
    }

    // LOGIN PHASE
    // RPC called by the server to tell this client what their player id is
    public void SetPlayerId(int aPlayerId)
    {
        Debug.Log("(" + aPlayerId + ") : has just Connected");
        playerId = aPlayerId;
        players[playerId] = new Player();
    }

    // RPC called by the server to tell this client which team they are on
    public void SetTeam(int team)
    {
        players[playerId].team = team;
    }

    public void NewPlayerConnected(int playerId, int team) // who connected
    {
        players[playerId] = new Player();
        players[playerId].team = team;
    }

    public void PlayerNameChanged(int playerId, string name) // changed player name
    {
        players[playerId].name = name;
    }

    public void PlayerIsReady(int playerId, bool isReady) // player is ready
    {
        Debug.Log(playerId + ": is ready");
        isReady = true;
        players[playerId].isready = isReady;
    }

    public void PlayerClassChanged(int playerId, int type) // player class changed
    {
        players[playerId].characterClass = type;
        if (type == 1)
        {
            players[playerId].playerObject = warrior;
            players[playerId].health = 100;
        }
        if (type == 2)
        {
            players[playerId].playerObject = rogue;
            players[playerId].health = 70;
        }
        if (type == 3)
        {
            players[playerId].playerObject = wizzard;
            players[playerId].health = 30;
        }
    }

    public void GameStart(int time) //Start Timer 
    {
        float timer = time;
        timer -= Time.deltaTime;
        gameStartTimer.text = timer.ToString();
    }




    // OUTGOING RPCS

    public void SetName(string name)
    {
        clientNet.CallRPC("SetName", UCNetwork.MessageReceiver.ServerOnly, -1, name); //Set Name Of Player
    }
    public void SetCharacterType(int type)
    {
        clientNet.CallRPC("SetCharacterType", UCNetwork.MessageReceiver.ServerOnly, -1, type); //Set Class Of Player
    }
    public void Ready(bool isReady)
    {
        clientNet.CallRPC("Ready", UCNetwork.MessageReceiver.ServerOnly, -1, isReady); // Is Player ready?
    }



    // GAME PHASE

    //Messages from the client to the server:

    //Outgoing RPCS

    public void RequestMove(int x, int y)
    {
        clientNet.CallRPC("RequestMove", UCNetwork.MessageReceiver.ServerOnly, -1, x, y); // RPC for moving
    }

    public void RequestAttack(int x, int y) 
    {
        clientNet.CallRPC("RequestAttack", UCNetwork.MessageReceiver.ServerOnly, -1, x, y); // RPC for attacking
    }

    public void SendChat(string message)
    {
        message = chat.text;
        clientNet.CallRPC("SendChat", UCNetwork.MessageReceiver.ServerOnly, -1, message); // RPC for sending a chat
    }

    public void SendTeamChat(string message)
    {
        message = chat.text;
        clientNet.CallRPC("SendTeamChat", UCNetwork.MessageReceiver.ServerOnly, -1, message); // RPC for sending a Team chat
    }

    public void PassTurn()
    {
        clientNet.CallRPC("PassTurn", UCNetwork.MessageReceiver.ServerOnly, -1); // Pass Turn
    }


    //Messages from the server to the client:

    public void SetMapSize(int x, int y) // How Large is the Map
    {
        int temp = 0;
        mapWidth = y;
        mapHeight = x;
        board = new GameObject[x * y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                currHeight = i; //Positions
                currWidth = j;
                board[temp] = Instantiate(movable, new Vector3(i * 2.6f, 0, j * 2.6f), Quaternion.identity);
                temp++;
            }
        }
        readyScreen.SetActive(false); // change cameras
        playScreen.SetActive(true);
        Camera.main.fieldOfView = x + y; clientNet.AddToArea(1);
        gameCam.transform.position = new Vector3(53 + x - 4.75f , 63 , y); //camera math so its center
    }

    public void SetBlockedSpace(int x, int y) // Set A Blocked Space
    {
        //Destroy(board[x * y]);
        board[x * y] = Instantiate(blocked, new Vector3(x * 2.6f, 0.001f, y * 2.6f), Quaternion.identity);
    }

    public void SetPlayerPosition(int playerId, int x, int y) // Set A Player Position
    {
        if (x == 0 || y == 0)
        {
            if (x != 0 && y == 0)
            {
                Instantiate(warrior, board[x * mapWidth].transform.position, Quaternion.identity); // first row Exception
            }
            if (x == 0 && y != 0)
            {
                Instantiate(warrior, board[y].transform.position, Quaternion.identity); // first column Exception
            }
        }
        if (x != 0 && y != 0)
        {
            int tempy;
            Debug.Log(mapWidth);
            tempy = x * mapWidth;
            Debug.Log(tempy);
            Instantiate(warrior, board[y + tempy].transform.position, Quaternion.identity); // anything else
        }
    }

    public void StartTurn(int playerId) // Start Turn
    {
        if (this.playerId == playerId)
        {
            playerTurn.text = "Your Turn"; // Change UI for Your Turn
        }
        if (this.playerId != playerId)
        {
            playerTurn.text = "It Is Not Your Turn"; // Change UI not your turn
        }
        players[playerId].isTurn = true;
    }

    public void AttackMade(int playerId, int x, int y) // Invalid
    {

    }

    public void DisplayChatMessage(string message) // Chat Message
    {
        chatBox.text = message;
    }

    public void UpdateHealth(int playerId, int newHealth) // Change Health
    {
        health.text = newHealth.ToString();
        /*health.text = players[playerId].health.ToString();
        if (players[playerId].health <= 0)
        {
            Destroy(players[playerId].playerObject);
        }*/
    }
}
