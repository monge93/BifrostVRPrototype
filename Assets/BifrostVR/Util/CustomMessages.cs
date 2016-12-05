using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Bifrost
{
    public class KeyboardInputMessage : MessageBase
    {
        public short key;
        public byte state;

        public override void Deserialize(NetworkReader reader)
        {
            key = reader.ReadInt16();
            state = reader.ReadByte();
        }
        
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(key);
            writer.Write(state);
        }

    }

    public class MousePositionMessage : MessageBase
    {
        public Vector2 position;

        public override void Deserialize(NetworkReader reader)
        {
            position = reader.ReadVector2();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(position);
        }

    }

    public class MouseButtonMessage : MessageBase
    {
        public short button;
        public byte state;

        public override void Deserialize(NetworkReader reader)
        {
            button = reader.ReadInt16();
            state = reader.ReadByte();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(button);
            writer.Write(state);
        }
    }

    public class MovementAxisMessage : MessageBase
    {
        public float value;

        public override void Deserialize(NetworkReader reader)
        {
            value = reader.ReadSingle();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(value);
        }
    }
}
