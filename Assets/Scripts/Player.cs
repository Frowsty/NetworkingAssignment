using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public NetworkVariable<int> m_MaxHealth = new NetworkVariable<int>(100);
    public NetworkVariable<int> m_CurrentHealth = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    private TextMeshProUGUI textMeshPro;
    
    ulong hostID = ulong.MinValue;

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("SendMessage", ReceiveMessage);

        if (IsHost)
            hostID = NetworkObjectId;
        
        if (IsOwner)
        {
            m_CurrentHealth.Value = m_MaxHealth.Value;
            TilemapManager.Instance.player = this;
            TilemapManager.Instance.GenerateTiles();
            
            Debug.Log("I'm the owner");
        }
        textMeshPro = GameObject.FindFirstObjectByType<TextMeshProUGUI>();
        Debug.Log("Player spawned");
    }

    public void ReceiveMessage(ulong senderId, FastBufferReader reader)
    {
        reader.ReadValueSafe(out string data);
        string[] splitData = data.Split('-');

        if (splitData.Length != 2) return;
        
        Int32.TryParse(splitData[0], out int senderID);
        
        Debug.Log("Player: " + senderID + " -> sent message: " + splitData[1]);
        
        textMeshPro.text = "Client " + senderID + " sent: " + splitData[1];

        StartCoroutine(RemoveTextFromScreen());
    }

    private void SendMessage()
    {
        string message = new string(NetworkObjectId + "-Hello");
        
        var manager = NetworkManager.Singleton.CustomMessagingManager;
        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(message), Allocator.Temp);

        using (writer)
        {
            writer.WriteValueSafe(message);
            
            textMeshPro.text = "You sent: " + message.Split("-")[1];

            if (IsHost)
                manager.SendNamedMessage("SendMessage", NetworkManager.Singleton.ConnectedClientsIds[NetworkManager.Singleton.ConnectedClientsIds.Count - 1],writer, NetworkDelivery.Reliable);
            else
                manager.SendNamedMessage("SendMessage", hostID, writer, NetworkDelivery.Reliable);
        }
    }

    IEnumerator RemoveTextFromScreen()
    {
        yield return new WaitForSeconds(2f);

        textMeshPro.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (IsClient && IsOwner))
            m_CurrentHealth.Value -= 10;

        if (Input.GetKeyDown(KeyCode.T) && IsOwner)
        {
            SendMessage();
            StartCoroutine(RemoveTextFromScreen());
        }
    }
}
