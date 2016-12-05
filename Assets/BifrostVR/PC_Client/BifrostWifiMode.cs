using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

namespace Bifrost
{
    public class BifrostWifiMode : BifrostNetwork
    {
        string _address;
        public int _port = 44777;
        NetworkClient _networkClient;
        
        void Start()
        {
            _networkClient = new NetworkClient();
            _networkClient.RegisterHandler(MsgType.Connect, OnConnected);
            _networkClient.RegisterHandler(MsgType.Disconnect, OnDisconnected);

            gameObject.AddComponent<BifrostNetworkDiscovery>();
        }

        void OnConnected(NetworkMessage netMsg)
        {
            GetComponent<BifrostNetworkDiscovery>().StopBroadcast();
            connected = true;
            if (_OnConnected != null)
                _OnConnected();
        }

        void OnDisconnected(NetworkMessage netMsg)
        {
            connected = false;
            if (_OnDisconnected != null)
                _OnDisconnected();
        }
                
        public void StartClient(string address)
        {
            //Log("Starting client");
            _address = address;
            StartCoroutine(Connect(address));
        }

        IEnumerator Connect(string address)
        {
            GetComponent<BifrostInputPC>().Log("Trying to connect.");
            _networkClient.Connect(address, _port);
            int attempts = 1;

            yield return new WaitForSeconds(3);
            while (!_networkClient.isConnected && attempts < 4)
            {
                if (!_networkClient.isConnected)
                {
                    GetComponent<BifrostInputPC>().Log("Trying to connect...");
                    _networkClient.Connect(address, _port);
                    attempts++;
                }
                yield return new WaitForSeconds(3);
            }

            if (!_networkClient.isConnected && _OnConnectFailed != null)
            {
                GetComponent<BifrostInputPC>().Log("Connect Failed.");
                _OnConnectFailed();
            }
        }

        public override void SendReliable(short msgType, MessageBase msg)
        {
            //_networkClient.Send(msgType, msg);
            _networkClient.SendByChannel(msgType, msg, UnityEngine.Networking.Channels.DefaultReliable);
        }

        public override void SendUnreliable(short msgType, MessageBase msg)
        {
            //_networkClient.Send(msgType, msg);
            _networkClient.SendByChannel(msgType, msg, UnityEngine.Networking.Channels.DefaultUnreliable);
        }

        public override void Connect()
        {
            if (_address == "")
                Debug.LogError("address not initialized");
            else
                StartCoroutine(Connect(_address));
        }

        public override void Disconnect()
        {
            
        }
    }
}
