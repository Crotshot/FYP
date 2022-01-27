using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour
{
    [SerializeField][SyncVar] private ulong ID;

    private void Start() {
        if (!isServer)
            return;

        ID = FindObjectOfType<GameStarter>().NextID();
    }

    public ulong GetID() {
        return ID;
    }
}
