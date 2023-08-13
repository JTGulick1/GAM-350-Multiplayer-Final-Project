using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

public class ExampleServer : MonoBehaviour
{
    public static ExampleServer instance;

    public ServerNetwork serverNet;

    public int portNumber = 603;
    public int totalPlayers = 8;
    public int readyPlayers = 0;
    public int ID = 1;
    public int role;
    int iterations = 0;
    int vote1 = 0;
    int vote2 = 0;
    int vote3 = 0;
    int vote4 = 0;
    int vote5 = 0;
    int vote6 = 0;
    int vote7 = 0;
    int vote8 = 0;

    //player status
    bool playeralive1 = true;
    bool playeralive2 = true;
    bool playeralive3 = true;
    bool playeralive4 = true;
    bool playeralive5 = true;
    bool playeralive6 = true;
    bool playeralive7 = true;
    bool playeralive8 = true;

    bool playerPortect1 = false;
    bool playerPortect2 = false;
    bool playerPortect3 = false;
    bool playerPortect4 = false;
    bool playerPortect5 = false;
    bool playerPortect6 = false;
    bool playerPortect7 = false;
    bool playerPortect8 = false;
    public bool isDay = false;

    // Stores a player
    class Player
    {
        public long clientId;
        public string playerName;
        public bool isReady;
        public bool isConnected;
        public int role = 0;
        public int num = 1;
    }
    List<Player> players = new List<Player>();
    int currentActivePlayer;

    // Use this for initialization
    void Awake()
    {
        instance = this;

        // Initialization of the server network
        ServerNetwork.port = portNumber;
        if (serverNet == null)
        {
            serverNet = GetComponent<ServerNetwork>();
        }
        if (serverNet == null)
        {
            serverNet = (ServerNetwork)gameObject.AddComponent(typeof(ServerNetwork));
            Debug.Log("ServerNetwork component added.");
        }

        //serverNet.EnableLogging("rpcLog.txt");
    }
    // A client has just requested to connect to the server
    void ConnectionRequest(ServerNetwork.ConnectionRequestInfo data)
    {
        Debug.Log("Connection request from " + data.username);

        // We either need to approve a connection or deny it
        //if (players.Count < 2)
        {
            Player newPlayer = new Player();
            newPlayer.clientId = data.id;
            newPlayer.playerName = data.username;
            newPlayer.isConnected = false;
            newPlayer.isReady = false;
            players.Add(newPlayer);
            serverNet.ConnectionApproved(data.id);
        }
    }
    public void GetPlayerID()
    {
        serverNet.CallRPC("RecieveID", serverNet.SendingClientId, -1, ID);
        ID++;
    }

    void OnClientConnected(long aClientId)
    {
        // Set the isConnected to true on the player
        foreach (Player p in players)
        {
            if (p.clientId == aClientId)
            {
                p.isConnected = true;
            }
        }
    }

    public void PlayerIsReady()
    {
        // Who called this RPC: serverNet.SendingClientId
        Debug.Log("Player is ready");

        // Set the isConnected to true on the player
        foreach (Player p in players)
        {
            if (p.clientId == serverNet.SendingClientId)
            {
                p.isReady = true;
            }
        }

        // Are all of the players ready?
        bool allPlayersReady = true;
        foreach (Player p in players)
        {
            if (!p.isReady)
            {
                allPlayersReady = false;
            }
        }
        if (allPlayersReady)
        {
            serverNet.CallRPC("GameStart", UCNetwork.MessageReceiver.AllClients, -1);
            totalPlayers = players.Count;
            AssignRoles();
        }
    }
    void OnClientDisconnected(long aClientId)
    {
        // Set the isConnected to true on the player
        foreach (Player p in players)
        {
            if (p.clientId == aClientId)
            {
                p.isConnected = false;
                p.isReady = false;
            }
        }
    }
    void AssignRoles()
    {
        foreach (Player p in players)
        {
            role = UnityEngine.Random.Range(1, 9);
            while (role != p.role && p.role != 0)
            {
                role = UnityEngine.Random.Range(1, 9);
            }
            p.role = role;
            serverNet.CallRPC("RoleAssigment", p.clientId, -1, role);
        }
        serverNet.CallRPC("TimerCountdown", UCNetwork.MessageReceiver.AllClients, -1, 5.0f);
    }

    public void TimeChange(bool isday)
    {
        if (isday == false && iterations == 0)
        {
            readyPlayers = 0;
            isDay = true;
            iterations++;
            serverNet.CallRPC("Day", UCNetwork.MessageReceiver.AllClients, -1);
            serverNet.CallRPC("TimerCountdown", UCNetwork.MessageReceiver.AllClients, -1, 60.0f);
        }
        if (isday == true && iterations == 0)
        {
            isDay = false;
            serverNet.CallRPC("Night", UCNetwork.MessageReceiver.AllClients, -1);
            vote1 = 0;
            vote2 = 0;
            vote3 = 0;
            vote4 = 0;
            vote5 = 0;
            vote6 = 0;
            vote7 = 0;
            vote8 = 0;
        }
    }
    public void ResetIts()
    {
        iterations = 0;
    }
    public void VoteCast(int playerNum)
    {
        if (playerNum == 1)
        {
            vote1++;
        }
        if (playerNum == 2)
        {
            vote2++;
        }
        if (playerNum == 3)
        {
            vote3++;
        }
        if (playerNum == 4)
        {
            vote4++;
        }
        if (playerNum == 5)
        {
            vote5++;
        }
        if (playerNum == 6)
        {
            vote6++;
        }
        if (playerNum == 7)
        {
            vote7++;
        }
        if (playerNum == 8)
        {
            vote8++;
        }
    }
    public void VoteCount()
    {
        checkStatus();
        TimeChange(true);
    }
    public void KillCast(int playernum)
    {
        if (playernum == 1)
        {
            playeralive1 = false;
        }
        if (playernum == 2)
        {
            playeralive2 = false;
        }
        if (playernum == 3)
        {
            playeralive3 = false;
        }
        if (playernum == 4)
        {
            playeralive4 = false;
        }
        if (playernum == 5)
        {
            playeralive5 = false;
        }
        if (playernum == 6)
        {
            playeralive6 = false;
        }
        if (playernum == 7)
        {
            playeralive7 = false;
        }
        if (playernum == 8)
        {
            playeralive8 = false;
        }
    }
    public void HealCast(int playernum)
    {
        if (playernum == 1 && playeralive1 == false)
        {
            playeralive1 = true;
        }
        if (playernum == 2 && playeralive2 == false)
        {
            playeralive2 = true;
        }
        if (playernum == 3 && playeralive3 == false)
        {
            playeralive3 = true;
        }
        if (playernum == 4 && playeralive4 == false)
        {
            playeralive4 = true;
        }
        if (playernum == 5 && playeralive5 == false)
        {
            playeralive5 = true;
        }
        if (playernum == 6 && playeralive6 == false)
        {
            playeralive6 = true;
        }
        if (playernum == 7 && playeralive7 == false)
        {
            playeralive7 = true;
        }
        if (playernum == 8 && playeralive8 == false)
        {
            playeralive8 = true;
        }
    }
    public void ProtectCast(int playernum)
    {
        if (playernum == 1 && playeralive1 == false)
        {
            playeralive1 = true;
            playerPortect1 = true;
        }
        if (playernum == 2 && playeralive2 == false)
        {
            playeralive2 = true;
            playerPortect2 = true;
        }
        if (playernum == 3 && playeralive3 == false)
        {
            playeralive3 = true;
            playerPortect3 = true;
        }
        if (playernum == 4 && playeralive4 == false)
        {
            playeralive4 = true;
            playerPortect4 = true;
        }
        if (playernum == 5 && playeralive5 == false)
        {
            playeralive5 = true;
            playerPortect5 = true;
        }
        if (playernum == 6 && playeralive6 == false)
        {
            playeralive6 = true;
            playerPortect6 = true;
        }
        if (playernum == 7 && playeralive7 == false)
        {
            playeralive7 = true;
            playerPortect7 = true;
        }
        if (playernum == 8 && playeralive8 == false)
        {
            playeralive8 = true;
            playerPortect8 = true;
        }
    }
    public void DistractCast(int playernum)
    {
        if (playernum == 1)
        {
            playeralive1 = true;
        }
        if (playernum == 2)
        {
            playeralive2 = true;
        }
        if (playernum == 3)
        {
            playeralive3 = true;
        }
        if (playernum == 4)
        {
            playeralive4 = true;
        }
        if (playernum == 5)
        {
            playeralive5 = true;
        }
        if (playernum == 6)
        {
            playeralive6 = true;
        }
        if (playernum == 7)
        {
            playeralive7 = true;
        }
        if (playernum == 8)
        {
            playeralive8 = true;
        }
    }
    public void SpyCast(int playernum)
    {
        if (playernum == 1 && playeralive1 == false)
        {

        }
        if (playernum == 2 && playeralive2 == false)
        {
        }
        if (playernum == 3 && playeralive3 == false)
        {

        }
        if (playernum == 4 && playeralive4 == false)
        {

        }
        if (playernum == 5 && playeralive5 == false)
        {

        }
        if (playernum == 6 && playeralive6 == false)
        {

        }
        if (playernum == 7 && playeralive7 == false)
        {

        }
        if (playernum == 8 && playeralive8 == false)
        {

        }
    }
    public void InfultrateCast(int playernum)
    {
        if (playernum == 1 && playerPortect1 == true)
        {
            playeralive1 = false;
        }
        if (playernum == 2 && playerPortect2 == true)
        {
            playeralive2 = false;
        }
        if (playernum == 3 && playerPortect3 == true)
        {
            playeralive3 = false;
        }
        if (playernum == 4 && playerPortect4 == true)
        {
            playeralive4 = false;
        }
        if (playernum == 5 && playerPortect5 == true)
        {
            playeralive5 = false;
        }
        if (playernum == 6 && playerPortect6 == true)
        {
            playeralive6 = false;
        }
        if (playernum == 7 && playerPortect7 == true)
        {
            playeralive7 = false;
        }
        if (playernum == 8 && playerPortect8 == true)
        {
            playeralive8 = false;
        }
    }
    public void isWaiting()
    {
        readyPlayers++;
        if (readyPlayers == totalPlayers)
        {
            checkStatus();
            TimeChange(false);
        }
    }
    public void checkStatus()
    {
        if (playeralive1 == false || vote1 >= 4)
        {
            serverNet.CallRPC("UnalivePlayer", players[0].clientId, -1);
        }
        if (playeralive2 == false || vote2 >= 4)
        {
            serverNet.CallRPC("UnalivePlayer", players[1].clientId, -1);
        }
        if (playeralive3 == false || vote3 >= 4)
        {
            serverNet.CallRPC("UnalivePlayer", players[2].clientId, -1);
        }
        if (playeralive4 == false || vote4 >= 4)
        {
            serverNet.CallRPC("UnalivePlayer", players[3].clientId, -1);
        }
        if (playeralive5 == false || vote5 >= 4)
        {
            serverNet.CallRPC("UnalivePlayer", players[4].clientId, -1);
        }
        if (playeralive6 == false || vote6 >= 4)
        {
            serverNet.CallRPC("UnalivePlayer", players[5].clientId, -1);
        }
        if (playeralive7 == false || vote7 >= 4)
        {
            serverNet.CallRPC("UnalivePlayer", players[6].clientId, -1);
        }
        if (playeralive8 == false || vote8 >= 4)
        {
            serverNet.CallRPC("UnalivePlayer", players[7].clientId, -1);
        }
    }
    public void FoundKiller()
    {
        serverNet.CallRPC("GameCompleted", UCNetwork.MessageReceiver.AllClients, -1);
    }
}