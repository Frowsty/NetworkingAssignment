using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

public class Monster : NetworkBehaviour
{
    private ObjectPool<Monster> monster_pool;
    private Player[] players;
    private MonsterSpawner monster_spawner;

    public int monsterID = 0;
    
    public int health;
    public int current_health = 0;
    public int damage = 3;

    void Start()
    {      
        monster_spawner = GameObject.Find("MonsterSpawner").GetComponent<MonsterSpawner>();
        players = GameObject.FindObjectsByType<Player>(FindObjectsSortMode.None);
        
        if (!monster_spawner)
            Debug.Log("Monster spawner missing");
    }
    
    
    private void onDeath()
    {
        monster_spawner.RemoveMonster(monsterID);
        monster_pool.Release(this);
        monster_spawner.decreaseSpawnedMonsters();
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
}
