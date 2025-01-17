using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

public class Monster : NetworkBehaviour
{
    private ObjectPool<Monster> monster_pool;
    private Player[] players;
    
    public int monsterID = 0;
    
    public int health;
    public int current_health = 0;
    public int damage = 3;
    
    public NetworkObject myNetworkObject;

    void Start()
    {
        players = GameObject.FindObjectsByType<Player>(FindObjectsSortMode.None);
        myNetworkObject = GetComponent<NetworkObject>();
    }
    
    
    private void onDeath()
    {
        monster_pool.Release(this);
        myNetworkObject.Despawn(false);
    }
    
    public void takeDamage(int damage)
    {
        current_health -= damage;

        if (current_health <= 0)
            onDeath();
    }
    
    public void setPool(ObjectPool<Monster> pool) => monster_pool = pool;
    
    public int doDamage()
    {
        return damage;
    }
    
    public void setHealth(int value) => current_health = value;
    public int getMaxHealth() => health;

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision detected");
    }
}
