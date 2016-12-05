using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Bifrost
{
    public class BifrostInputPC : MonoBehaviour
    {
        [SerializeField] Text _logText;
        [SerializeField] BifrostNetwork _network;

        Transform mouse_transform;

        Vector2 mouse_position;
        float verticalAxis, horizontalAxis;

        float mouse_position_sendrate = 1f / 15f;
        float movement_axis_sendrate = 1f / 10f;

        float mouse_position_sendtimer = 0;
        float horizontal_axis_sendtimer = 0;
        float vertical_axis_sendtimer = 0;

        [SerializeField] GameObject _modeSelectPanel;
        
        void Awake()
        {
            Application.runInBackground = true;
            InitializeWiFiMode();
            mouse_transform = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        }
        
        public void Log(string text)
        {
            if(_logText != null)
                _logText.text += "\n" + text;
            Debug.Log(text);
        }
/// INITIALIZE
        public void InitializeWiFiMode()
        {
            _modeSelectPanel.SetActive(false);
            _network = gameObject.AddComponent<BifrostWifiMode>();
            _network._OnConnected += OnConnected;
            _network._OnConnectFailed += OnConnectFailed;
            _network._OnDisconnected += OnDisconnected;
        }

        public void InitializeBluetoothMode()
        {

        }
        
/// SEND KEYBOARD EVENTS
        void OnGUI()
        {
            Event e = Event.current;

            if (_network != null && _network.connected && e != null)
            {
                if (e.isKey)
                {
                    bool keyUp = UnityEngine.Input.GetKeyUp(e.keyCode);
                    bool keyDown = UnityEngine.Input.GetKeyDown(e.keyCode);

                    if (e.keyCode != KeyCode.None && (keyDown || keyUp))
                    {
                        KeyboardInputMessage km = new KeyboardInputMessage();
                        km.key = KeyCodeToShort.GetShort(e.keyCode);

                        if (keyDown)
                            km.state = KeyState.Down;
                        else
                            km.state = KeyState.Up;

                        //_networkClient.Send(CustomMsgType.KeyboardInput, km);
                        _network.SendReliable(CustomMsgType.KeyboardInput, km);
                    }

                }
                else if(e.isMouse && e.clickCount != 0)
                {
                    MouseButtonMessage bm = new MouseButtonMessage();
                    bm.button = (short)e.button;
                    bool butDown = UnityEngine.Input.GetMouseButtonDown(bm.button);
                    if (butDown)
                        bm.state = KeyState.Down;
                    else
                        bm.state = KeyState.Up;
                    //_networkClient.Send(CustomMsgType.MouseButton, bm);
                    _network.SendReliable(CustomMsgType.MouseButton, bm);
                }
            }
            
        }
        
        void Update()
        {
            if (_network == null || !_network.connected)
                return;

            //CHECK MOUSE CHANGES
            mouse_position_sendtimer += Time.deltaTime;
            Vector2 new_mouse_position = mouse_transform.position;

            //if (Vector2.Distance(new_mouse_position, mouse_position) > 1f)
            {
                if (mouse_position_sendtimer > mouse_position_sendrate)
                {
                    MousePositionMessage mm = new MousePositionMessage();
                    mm.position = new_mouse_position;
                    _network.SendReliable(CustomMsgType.MouseInput, mm);
                    mouse_position = new_mouse_position;
                    mouse_position_sendtimer = 0;
                }
            }

            //CHECK HORIZONTAL AXIS CHANGE
            horizontal_axis_sendtimer += Time.deltaTime;
            float hor_axis = UnityEngine.Input.GetAxis("Horizontal");
            if (Mathf.Abs(hor_axis - horizontalAxis) > 0.04f && horizontal_axis_sendtimer > movement_axis_sendrate)
            {
                horizontalAxis = hor_axis;
                MovementAxisMessage am = new MovementAxisMessage();
                am.value = hor_axis;
                _network.SendReliable(CustomMsgType.HorizontalAxis, am);
                horizontal_axis_sendtimer = 0;
            }

            //CHECK VERTICAL AXIS CHANGE
            vertical_axis_sendtimer += Time.deltaTime;
            float ver_axis = UnityEngine.Input.GetAxis("Vertical");
            if (Mathf.Abs(ver_axis - verticalAxis) > 0.04f && vertical_axis_sendtimer > movement_axis_sendrate)
            {
                verticalAxis = ver_axis;
                MovementAxisMessage am = new MovementAxisMessage();
                am.value = ver_axis;
                _network.SendReliable(CustomMsgType.VerticalAxis, am);
                vertical_axis_sendtimer = 0;
            }

            //MOVE THE MOUSE REFERENCE TRANSFORM
            mouse_transform.position += new Vector3(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));
        }

/// LISTENERS
        void OnConnected()
        {
            Log("Succesfully connected to smartphone.");
        }

        void OnConnectFailed()
        {
            Log("Connect failed. Trying again.");
            _network.Connect();
        }

        void OnDisconnected()
        {
            Log("Disconnected from smartphone. Trying to reconnect.");
            _network.Connect();
        }
    }

    public class KeyCodeToShort
    {
        public static short GetShort(KeyCode key)
        {
            return (short)key;
        }
    }
}
