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
    int points; // points given

    // Stores a player
    class Player
    {
        public long clientId;
        public string playerName;
        public bool isReady;
        public bool isConnected;
        public int points;
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
            // Tell the first player it's their turn
            currentActivePlayer = 0;
            int housePTS = UnityEngine.Random.Range(16, 20);
            serverNet.CallRPC("HouseSet", UCNetwork.MessageReceiver.AllClients, -1, housePTS); // start the game
            serverNet.CallRPC("StartTurn", players[0].clientId, -1); // player 1 goes first
            serverNet.CallRPC("StartRound", UCNetwork.MessageReceiver.AllClients, -1, players.Count);
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
    public void Hit() // give player points
    {
        points = 0;
        points = UnityEngine.Random.Range(2, 11);
        serverNet.CallRPC("Addpoints", UCNetwork.MessageReceiver.AllClients, -1, currentActivePlayer, points);
    }
    public void NextTurn() // next turn
    {
        currentActivePlayer++;
        if (currentActivePlayer > players.Count)
        {
            currentActivePlayer = 1;
        }
        serverNet.CallRPC("StartTurn", players[currentActivePlayer - 1].clientId, -1);
    }
    public void PlayerCount() // How many players are in the game
    {
        serverNet.CallRPC("GetPlayerCount", UCNetwork.MessageReceiver.AllClients, -1, players.Count);
    }
    public void Reset() // restart the game
    {
        serverNet.CallRPC("RestartGame", UCNetwork.MessageReceiver.AllClients, -1);
    }
    public void HouseReset() // change house score
    {
        int housePTS = UnityEngine.Random.Range(14, 20);
        serverNet.CallRPC("HouseReset", UCNetwork.MessageReceiver.AllClients, -1, housePTS);
    }
}