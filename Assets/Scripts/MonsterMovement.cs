using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MonsterMovement : NetworkBehaviour
{
    private Player[] players;
    private Player closestPlayer;
    private Rigidbody2D rb;
    
    private float distance = 0f;
    private float look_angle;
    public float CONST_MOVE_SPEED = 0.75f;

    private void Start()
    {
        players = GameObject.FindObjectsByType<Player>(FindObjectsSortMode.None);
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        foreach (Player p in players)
        {
            if (Vector3.Distance(p.transform.position, transform.position) < distance)
                closestPlayer = p;
            distance = Vector3.Distance(p.transform.position, transform.position);
        }
        Vector3 target_pos = closestPlayer.transform.position;
        Vector3 rotation = target_pos - transform.position;
        
        look_angle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        rb.rotation = look_angle;

        if (Vector3.Distance(transform.position, target_pos) < 14f)
            rb.linearVelocity = transform.right * CONST_MOVE_SPEED;

        if (Vector3.Distance(transform.position, target_pos) < 2f && closestPlayer.IsOwner)
            closestPlayer.m_MaxHealth.Value -= 10;
    }
}
