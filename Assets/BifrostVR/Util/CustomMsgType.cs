using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Bifrost
{
    public class CustomMsgType : MsgType
    {
        public static short KeyboardInput = 100;
        public static short MouseInput = 101;
        public static short MouseButton = 102;
        public static short HorizontalAxis = 103;
        public static short VerticalAxis = 104;
    }
}