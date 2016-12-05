using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Bifrost
{
    public class Input : MonoBehaviour
    {
        [SerializeField] Text _debugText;
        [SerializeField] GameObject _modeSelectPanel;

#region VARIABLES

        public int port = 44777;

        HashSet<KeyCode> keys_holding;
        HashSet<KeyCode> keys_down;
        HashSet<KeyCode> keys_up;
        List<int> mousebutton_holding;
        List<int> mousebutton_down;
        List<int> mousebutton_up;
        
        Vector2 _mousePosition;
        Vector2 mouseRawAxis;
        Vector2 movementAxis;

        static Input instance;

        bool connected;
#endregion

        void Awake()
        {
            Application.runInBackground = true;

            keys_holding = new HashSet<KeyCode>();
            keys_down = new HashSet<KeyCode>();
            keys_up = new HashSet<KeyCode>();
            mousebutton_down = new List<int>();
            mousebutton_up = new List<int>();
            mousebutton_holding = new List<int>();
            instance = this;

            NetworkServer.RegisterHandler(MsgType.Connect, OnConnected);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconnected);
            NetworkServer.RegisterHandler(CustomMsgType.KeyboardInput, OnKeyboardInputReceived);
            NetworkServer.RegisterHandler(CustomMsgType.MouseInput, OnMouseInputReceived);
            NetworkServer.RegisterHandler(CustomMsgType.MouseButton, OnMouseButtonInputReceived);
            NetworkServer.RegisterHandler(CustomMsgType.HorizontalAxis, OnHorizontalAxisReceived);
            NetworkServer.RegisterHandler(CustomMsgType.VerticalAxis, OnVerticalAxisReceived);
        }

#region HANDLERS

        void OnConnected(NetworkMessage netMsg)
        {
            GetComponent<BifrostNetworkDiscovery>().StopBroadcast();
            Log("Connected to PC.");
            connected = true;
        }

        void OnDisconnected(NetworkMessage netMsg)
        {
            Log("Connection lost. Trying to reconnect");
            connected = false;
        }

        void OnKeyboardInputReceived(NetworkMessage netMsg)
        {
            if (netMsg.reader != null)
            {
                KeyboardInputMessage km = netMsg.ReadMessage<KeyboardInputMessage>();
                
                KeyCode keypressed = ShortToKeyCode.GetKeyCode(km.key);

                if (km.state == KeyState.Down)
                {
                    keys_holding.Add(keypressed);
                    StartCoroutine(KeyDownRoutine(keypressed));
                }
                else if (km.state == KeyState.Up)
                {
                    keys_holding.Remove(keypressed);
                    StartCoroutine(KeyUpRoutine(keypressed));
                }
            }
        }

        void OnMouseInputReceived(NetworkMessage netMsg)
        {
            if (netMsg.reader != null)
            {
                MousePositionMessage mm = netMsg.ReadMessage<MousePositionMessage>();
                Vector2 new_mouse_position = mm.position;

                if (Vector2.Distance(new_mouse_position, _mousePosition) > 0.05f)
                {
                    mouseRawAxis.x = new_mouse_position.x - _mousePosition.x;
                    mouseRawAxis.y = new_mouse_position.y - _mousePosition.y;
                    _mousePosition = new_mouse_position;
                }
                else
                {
                    mouseRawAxis = Vector2.zero;
                }
            }
        }

        void OnMouseButtonInputReceived(NetworkMessage netMsg)
        {
            if (netMsg.reader != null)
            {
                MouseButtonMessage mb = netMsg.ReadMessage<MouseButtonMessage>();
                if(mb.state == KeyState.Down)
                {
                    mousebutton_holding.Add(mb.button);
                    StartCoroutine(MouseButtonDownRoutine(mb.button));
                }
                else if (mb.state == KeyState.Up)
                {
                    mousebutton_holding.Remove(mb.button);
                    StartCoroutine(MouseButtonUpRoutine(mb.button));
                }
            }
        }

        void OnHorizontalAxisReceived(NetworkMessage netMsg)
        {
            if (netMsg.reader != null)
            {
                MovementAxisMessage ma = netMsg.ReadMessage<MovementAxisMessage>();
                movementAxis.x = ma.value;
            }
        }

        void OnVerticalAxisReceived(NetworkMessage netMsg)
        {
            if (netMsg.reader != null)
            {
                MovementAxisMessage ma = netMsg.ReadMessage<MovementAxisMessage>();
                movementAxis.y = ma.value;
            }
        }

        #endregion

#region INPUT METHODS

        public IEnumerator KeyDownRoutine(KeyCode k)
        {
            keys_down.Add(k);
            yield return 0;
            keys_down.Remove(k);
        }
        public IEnumerator KeyUpRoutine(KeyCode k)
        {
            keys_up.Add(k);
            yield return 0;
            keys_up.Remove(k);
        }

        public IEnumerator MouseButtonDownRoutine(int button)
        {
            mousebutton_down.Add(button);
            yield return 0;
            mousebutton_down.Remove(button);
        }
        public IEnumerator MouseButtonUpRoutine(int button)
        {
            mousebutton_up.Add(button);
            yield return 0;
            mousebutton_up.Add(button);
        }

        public static bool GetKey(KeyCode key)
        {
            if (Input.instance.connected)
            {
                if (instance.keys_holding.Contains(key))
                 return true;
                return false;
            }
            else
                return UnityEngine.Input.GetKey(key);
        }
        public static bool GetKeyDown(KeyCode key)
        {
            if (Input.instance.connected)
            {
                if (instance.keys_down.Contains(key))
                    return true;
                return false;
            }
            else
                return UnityEngine.Input.GetKeyDown(key);
        }
        public static bool GetKeyUp(KeyCode key)
        {
            if (Input.instance.connected)
            {
                if (instance.keys_up.Contains(key))
                    return true;
                return false;
            }
            else
                return UnityEngine.Input.GetKeyUp(key);
        }

        public static Vector2 GetMouseAxis()
        {
            if (Input.instance.connected)
                return instance.mouseRawAxis;
            else
                return 
                    new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));
        }
        
        public static bool GetMouseButtonDown(int button)
        {
            if (Input.instance.connected)
            {
                if (instance.mousebutton_down.Contains(button))
                    return true;
                return false;
            }
            else
                return UnityEngine.Input.GetMouseButtonDown(button);
        }
        public static bool GetMouseButtonUp(int button)
        {
            if (Input.instance.connected)
            {
                if (instance.mousebutton_up.Contains(button))
                    return true;
                return false;
            }
            else
                return UnityEngine.Input.GetMouseButtonUp(button);
        }
        public static bool GetMouseButton(int button)
        {
            if (Input.instance.connected)
            {
                if (instance.mousebutton_holding.Contains(button))
                    return true;
                return false;
            }
            else
                return UnityEngine.Input.GetMouseButton(button);
        }

        public static float HorizontalAxis()
        {
            if (Input.instance.connected)
                return instance.movementAxis.x;
            else
                return UnityEngine.Input.GetAxis("Horizontal");
        }
        public static float VerticalAxis()
        {
            if (Input.instance.connected)
                return instance.movementAxis.y;
            else
                return UnityEngine.Input.GetAxis("Vertical");
        }

        #endregion

        public void Log(string text)
        {
            if (_debugText != null)
                _debugText.text += "\n" + text;

            Debug.Log(text);
        }

        public void _OnWifiModeClick()
        {
            _modeSelectPanel.gameObject.SetActive(false);
            StartCoroutine(StartHost());
        }

        public void _OnNoneClick()
        {
            _modeSelectPanel.gameObject.SetActive(false);
        }
        
        IEnumerator StartHost()
        {
            Log("Starting server...");
            NetworkServer.Listen(port);

            while (!NetworkServer.active)
            {
                Log("Error.  Trying again in 2 seconds.");
                yield return new WaitForSeconds(2);
                Log("Starting server.");
                if (!NetworkServer.active)
                    NetworkServer.Listen(port);
            }

            Log("Server initialized.");
            gameObject.AddComponent<BifrostNetworkDiscovery>();
        }
    }

#region UTILITY CLASSES

    public class KeyState
    {
        public static byte Down = 0; 
        public static byte Up = 1;
    }

    public class ShortToKeyCode
    {
        public static KeyCode GetKeyCode(short idx)
        {
            return (KeyCode)idx;
        }
    }

#endregion
}
