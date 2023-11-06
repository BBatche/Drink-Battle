using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkScript : NetworkBehaviour
{
    private float interpolationTime = 0.1f;
    private Vector2 velocity;
    private readonly NetworkVariable<PlayerNetworkData> networkState =
        new NetworkVariable<PlayerNetworkData>(writePerm: NetworkVariableWritePermission.Owner);

    struct PlayerNetworkData : INetworkSerializable
    {
        private float x;
        private float y;
        
        internal Vector2 Position
        {
            get => new Vector2(x, y);
            set { x = value.x; y = value.y; }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref x);
            serializer.SerializeValue(ref y);
        }
    }

        // Start is called before the first frame update
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            networkState.Value = new PlayerNetworkData()
            {
                Position = transform.position
            };
        }
        else
        {
           // transform.position = networkState.Value.Position;
            transform.position = Vector2.SmoothDamp(transform.position,networkState.Value.Position,ref velocity, interpolationTime );
        }
    }
}
