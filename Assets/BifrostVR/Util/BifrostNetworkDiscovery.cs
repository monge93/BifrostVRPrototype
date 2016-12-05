using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Bifrost
{
    public class BifrostNetworkDiscovery : NetworkDiscovery
    {
        bool connectionmade;

        // Use this for initialization
        void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            showGUI = false;
            Initialize();

            if (GetComponent<Input>() != null)
                GetComponent<Input>().Log("network discovery started as server: " + StartAsServer());
            else
                GetComponent<BifrostInputPC>().Log("network discovery started as client: " + StartAsClient());
        }

        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            if (connectionmade)
                return;

            connectionmade = true;
            
            GetComponent<BifrostInputPC>().Log("Received broadcast from " + fromAddress + ". Data: " + data);
            BifrostWifiMode ipt = gameObject.GetComponent<BifrostWifiMode>();

            if (ipt == null)
                return;

            GetComponent<BifrostInputPC>().Log("Connecting to smartphone");
            ipt.StartClient(fromAddress);

            //StopBroadcast();
        }
    }
}
