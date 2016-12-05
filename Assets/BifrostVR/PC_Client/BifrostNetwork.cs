using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Bifrost
{
    public abstract class BifrostNetwork : MonoBehaviour
    {
        public bool connected { protected set; get;}

        public abstract void SendReliable(short msgType, MessageBase msg);
        public abstract void SendUnreliable(short msgType, MessageBase msg);
        public abstract void Connect();
        public abstract void Disconnect();
        
    ////DELEGATES
        public delegate void OnConnectedDelegate();
        public delegate void OnDisconnectedDelegate();
        public delegate void OnConnectFailedDelegate();

        public OnConnectedDelegate _OnConnected;
        public OnDisconnectedDelegate _OnDisconnected;
        public OnConnectFailedDelegate _OnConnectFailed;
    }
}
