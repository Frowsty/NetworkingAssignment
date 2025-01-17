using System;
using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public NetworkVariable<int> m_MaxHealth = new NetworkVariable<int>(100);
    public NetworkVariable<int> m_CurrentHealth = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    private TextMeshProUGUI textMeshPro;

    private float lastDamageTaken = 0f;

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("SendMessage", ReceiveMessage);
        
        if (IsOwner)
        {
            m_CurrentHealth.Value = m_MaxHealth.Value;
            TilemapManager.Instance.player = this;
            TilemapManager.Instance.GenerateTiles();
        }
        textMeshPro = GameObject.FindFirstObjectByType<TextMeshProUGUI>();
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
    
            foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (clientID == OwnerClientId) continue; // don't send message to yourself
                manager.SendNamedMessage("SendMessage", clientID, writer, NetworkDelivery.Reliable);
            }
            
        }
    }

    IEnumerator RemoveTextFromScreen()
    {
        yield return new WaitForSeconds(2f);

        textMeshPro.text = "";
    }
    
    
    [ClientRpc]
    public void TakeDamageClientRpc(ulong clientID)
    {
        if (clientID != OwnerClientId || !IsOwner) return; 
        if (Time.time - lastDamageTaken < 1.5f) return;
        
        m_CurrentHealth.Value -= 5;
        lastDamageTaken = Time.time;
        
        if (m_CurrentHealth.Value <= 0)
            PlayerDeathServerRpc(clientID);
    }

    [ServerRpc]
    private void PlayerDeathServerRpc(ulong clientID)
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Key == clientID)
                client.Value.PlayerObject.Despawn();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && IsOwner)
        {
            SendMessage();
            StartCoroutine(RemoveTextFromScreen());
        }
    }
}
