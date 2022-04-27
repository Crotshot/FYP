using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class ChatBox : NetworkBehaviour { 
<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
    [SerializeField] TMP_InputField messageInput;
    [SerializeField] GameObject content, textMessagePrefab, chat;

    bool active = true;
    public void ToggleChat() {
        active ^= true;
        chat.SetActive(active);
    }

    private void Start() {
        messageInput.onSubmit.AddListener(Message);    
    }

    public void Message(string messageText) {
        messageInput.text = "";
        if (isServer) {
            RpcMessage(messageText);
        }
        else {
            CmdMessage(messageText);
        }
    }

    [ClientRpc]
    public void RpcMessage(string messageText) {
        GameObject message = Instantiate(textMessagePrefab, content.transform);
        message.GetComponent<TMP_Text>().text = messageText;
    }

<<<<<<< Updated upstream
    [Command]
=======
    [Command (requiresAuthority =false)]
>>>>>>> Stashed changes
    public void CmdMessage(string messageText) {
        Message(messageText);
    }

}
