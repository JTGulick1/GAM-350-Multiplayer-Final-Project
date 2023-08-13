using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

public class ExampleServer : MonoBehaviour
{
    public ServerNetwork serverNet;

    public int portNumber = 603;

    public GameObject playerObject;

    Dictionary<int, GameObject> playerGameObjects = new Dictionary<int, GameObject>();

    bool isSomeoneIt = false; // is someone it? 

    int currTagger = 1; // starts with first to join
    public int ID = 1; // ID for players

    // Initialize the server
    void Awake()
    {
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

    // CALLBACK FUNCTIONS
    //  The following functions will be called by the ServerNetwork script while the game is running:

    // A client has just requested to connect to the server
    void ConnectionRequest(ServerNetwork.ConnectionRequestInfo data)
    {
        Debug.Log("Connection request from " + data.username);

        // Approve the connection
        serverNet.ConnectionApproved(data.id);
        OnClientConnected(data.id);
    }

    // A client has finished connecting to the server
    void OnClientConnected(long aClientId)
    {
        Debug.Log("Player: " + aClientId + " Connected");
    }

    // A client has disconnected
    void OnClientDisconnected(long aClientId)
    {

    }

    // A network object has been instantiated by a client
    void OnInstantiateNetworkObject(ServerNetwork.IntantiateObjectData aObjectData)
    {
        // Get the network object information, store in a dictionary
        ServerNetwork.NetworkObject obj = serverNet.GetNetObjById(aObjectData.netObjId);
        playerGameObjects[aObjectData.netObjId] = Instantiate(playerObject, obj.position, obj.rotation);
    }

    // A client has been added to a new area
    void OnAddArea(ServerNetwork.AreaChangeInfo aInfo)
    {

    }

    // An object has been added to a new area
    void AddedObjectToArea(int aNetworkId)
    {

    }

    // Initialization data should be sent to a network object
    void InitializeNetworkObject(ServerNetwork.InitializationInfo aInfo)
    {

    }

    // A game object has been destroyed
    void OnDestroyNetworkObject(int aObjectId)
    {

    }

    private void Update()
    {
        if (isSomeoneIt) // if someone is it get the location of everyone
        {
            Dictionary<int, ServerNetwork.NetworkObject> allObjs = serverNet.GetAllObjects();
        }
        if (!isSomeoneIt && serverNet.GetAllClients().Count > 0) // if nobody is it make someone it
        {
            CreateTagger();
            //isSomeoneIt = true;
        }
        
    }

    public void NetObjectUpdated(int aNetId)
    {
        Debug.Log("Object has been updated: " + aNetId); // Debug Line

        ServerNetwork.NetworkObject obj = serverNet.GetNetObjById(aNetId);
        playerGameObjects[aNetId].transform.position = obj.position; // Get client's position
        playerGameObjects[aNetId].transform.localRotation = obj.rotation; // Get clients's rotation
        if (currTagger != aNetId) // we do not want the current tagger to tag it's self
        {
            if (Vector3.Distance(playerGameObjects[aNetId].transform.position , playerGameObjects[currTagger].transform.position) <= 1) // if the Distance between the tagger and other client is less then 1
            {
                PlayersTouching(aNetId); // change taggers
            }
        }
    }
    public void PlayersTouching(int aNetId)
    {
        Debug.Log("Players are touching");// Debug Line
        long ownerClientId = serverNet.GetOwnerClientId(aNetId);
        serverNet.CallRPC("GotTagged", UCNetwork.MessageReceiver.AllClients, currTagger, false);
        currTagger = aNetId;
        serverNet.CallRPC("GotTagged", UCNetwork.MessageReceiver.AllClients, aNetId, true);
    }
    public void CreateTagger() // If nobody is tagger or tagger Disconnects
    {
        long ownerClientId = serverNet.GetOwnerClientId(currTagger);
        serverNet.CallRPC("GotTagged", UCNetwork.MessageReceiver.AllClients, currTagger, true); 
    }
    public int GetID()
    {
        return ID; // Get Player ID (Check Player Script)
    }
}
