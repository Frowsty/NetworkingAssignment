using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    private string address = "127.0.0.1";

    [SerializeField] private NetworkTransport transport;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    private void OnGUI()
    {
        if (NetworkManager.Singleton == null)
            return;

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 14,
            fontStyle = FontStyle.Normal,
            padding = new RectOffset(8, 8, 5, 5)
        };

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        GUIStyle labelStyle2 = new GUIStyle(GUI.skin.label)
        {
            fontSize = 8,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter
        };

        float buttonHeight = 30;
        float buttonSpacing = 5;
        float labelHeight = 20;
        float totalHeight = labelHeight + (buttonHeight + buttonSpacing) * 4;
        float width = 160;

        float x = 10;
        float y = 10;

        GUI.Box(new Rect(x, y, width, totalHeight), GUIContent.none);

        GUILayout.BeginArea(new Rect(x, y, width, totalHeight));

        GUILayout.Label("Network Manager", labelStyle);
        GUILayout.Label("\"Q\" to toggle cursor", labelStyle2);
        GUILayout.Space(buttonSpacing);

        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            if (GUILayout.Button("Host", buttonStyle, GUILayout.Height(buttonHeight)))
            {
                if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsClient)
                {
                    NetworkManager.Singleton.StartHost();


                    Debug.Log("Hosting session...");
                }
            }

            if (GUILayout.Button("Join", buttonStyle, GUILayout.Height(buttonHeight)))
            {
                if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsClient)
                {
                    NetworkManager.Singleton.StartClient();
                    Debug.Log("Joining session...");
                }
            }

            address = GUILayout.TextField(address, GUILayout.Height(buttonHeight));
            UpdateTransport();
        }
        else
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
            {
                if (GUILayout.Button("Disconnect", buttonStyle, GUILayout.Height(buttonHeight)))
                {
                    NetworkManager.Singleton.Shutdown();
                    Debug.Log("Disconnected from session.");
                }
            }
        }

        GUILayout.EndArea();
    }

    private void UpdateTransport()
    {
        if (transport is UnityTransport unityTransport)
        {
            unityTransport.SetConnectionData(address, 7777);
        }
    }
}