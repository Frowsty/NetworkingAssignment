using Unity.Netcode;
using UnityEngine;

public class HealthBar : NetworkBehaviour
{
    [SerializeField]
    private Transform player;
    private Player playerHealth;
    private LineRenderer lineRendererHealth;
    private LineRenderer lineRendererHealthBackground;

    public override void OnNetworkSpawn()
    {
        playerHealth = GetComponent<Player>();
        
        LineRenderer[] lines = player.GetComponentsInChildren<LineRenderer>();
        lineRendererHealth = lines[0];
        lineRendererHealthBackground = lines[1];

        lineRendererHealth.positionCount = 2;
        lineRendererHealthBackground.positionCount = 2;
        lineRendererHealth.SetPositions(new[] { new Vector3(player.position.x - 0.5f, player.position.y + -0.5f, 0),
                                                new Vector3(player.position.x + 0.5f, player.position.y + -0.5f, 0) });
        lineRendererHealthBackground.SetPositions(new[] { new Vector3(player.position.x - 0.5f, player.position.y + -0.5f, 0),
                                                          new Vector3(player.position.x + 0.5f, player.position.y + -0.5f, 0) });
        
        lineRendererHealth.startWidth = 0.1f;
        lineRendererHealth.endWidth = 0.1f;
        lineRendererHealthBackground.startWidth = 0.1f;
        lineRendererHealthBackground.endWidth = 0.1f;
        
        lineRendererHealth.material = new Material(Shader.Find("Sprites/Default"));
        lineRendererHealth.startColor = new Color(0f, 1f, 0f, 1f);
        lineRendererHealth.endColor = new Color(0f, 1f, 0f, 1f);
        
        lineRendererHealthBackground.material = new Material(Shader.Find("Sprites/Default"));
        lineRendererHealthBackground.startColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        lineRendererHealthBackground.endColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    }

    private void Update()
    {
        float healthPercent = (float)playerHealth.m_CurrentHealth.Value / (float)playerHealth.m_MaxHealth.Value;
        healthPercent = healthPercent < 0 ? 0 : healthPercent;
        
        lineRendererHealth.SetPositions(new[] { new Vector3(player.position.x - 0.5f, player.position.y + -0.5f, 0),
                                                new Vector3(player.position.x - 0.5f + healthPercent, player.position.y + -0.5f, 0) });
        lineRendererHealthBackground.SetPositions(new[] { new Vector3(player.position.x - 0.5f, player.position.y + -0.5f, 0),
                                                          new Vector3(player.position.x + 0.5f, player.position.y + -0.5f, 0) });
    }
}