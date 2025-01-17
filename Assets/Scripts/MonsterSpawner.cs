using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class MonsterSpawner : NetworkBehaviour
{
    float base_spawn_delay = 10f;
    
    [SerializeField]
    private Monster monster_prefab;

    private CustomMessagingManager messagingManager;
    
    private Dictionary<int, Monster> monsters = new Dictionary<int, Monster>();
    
    private ObjectPool<Monster> monster_pool;
    
    private int max_spawn = 100;
    private int spawned_monsters = 0;
    private float spawn_timer = 0f;
    private bool did_spawn = false;

    private static int monsterID = 1;

    private bool should_spawn = false;

    public override void OnNetworkSpawn()
    {
        monster_pool = new ObjectPool<Monster>(createMonster, onTakeMonster, onReturnMonster, onDestroyMonster,
            true, 200, 1500);
    }

    void Update()
    {
        if (IsHost)
        {
            if (NetworkManager.Singleton.ConnectedClients.Count > 1)
                spawnMonsters();
        }
    }

    private Monster createMonster()
    {
        // create new instance
        Monster monster = Instantiate(monster_prefab, pointOutsideScreen(), Quaternion.identity);
        monster.setPool(monster_pool);
        
        return monster;
    }

    private void onTakeMonster(Monster monster)
    {
        monster.transform.position = pointOutsideScreen();
        monster.transform.rotation = Quaternion.identity;
        monster.setHealth(monster.getMaxHealth());
        
        monster.gameObject.SetActive(true);
    }

    private void onReturnMonster(Monster monster) => monster.gameObject.SetActive(false);

    private void onDestroyMonster(Monster monster) => Destroy(monster.gameObject);
    
    private Vector3 pointOutsideScreen()
    {
        float x = Random.Range(-0.2f, 0.2f);
        float y = Random.Range(-0.2f, 0.2f);
        if (x >= 0f) x += 1f;
        if (y >= 0f) y += 1f;
         
        Vector3 random_point = new Vector3(x, y);
        Vector3 world_point = Camera.main.ViewportToWorldPoint(random_point);

        world_point.z = -2;
        
        return world_point;
    }
    
    [ClientRpc]
    void RemoveMonsterClientRpc(int monsterID)
    {
        if (IsHost) return;
        
        Debug.Log("Removing Monster");
        
        monsters.TryGetValue(monsterID, out Monster m);
        if (m != null)
        {
            monster_pool.Release(m);
            monsters.Remove(monsterID);
        }
    }

    public void RemoveMonster(int monsterID)
    {
        if (!IsHost) return;
        
        RemoveMonsterClientRpc(monsterID);
    }

    [ClientRpc]
    void SpawnMonsterClientRpc(Vector4 data)
    {
        if (IsHost) return;
        
        Monster m = monster_pool.Get();
        m.monsterID = (int)data[3];
        m.transform.position = new Vector3(data[0], data[1], data[2]);
        
        monsters.Add(m.monsterID, m);
    }

    private void spawnMonsters()
    {
        if (spawned_monsters < max_spawn && !did_spawn)
        {
            var m = monster_pool.Get();
            m.monsterID = monsterID;
            SpawnMonsterClientRpc(new Vector4(m.transform.position.x, m.transform.position.y, m.transform.position.z, monsterID));
            increaseSpawnedMonsters();
            monsterID++;

            spawn_timer = 0f;
        }
        else
            did_spawn = true;

        if (spawn_timer >= get_spawn_time_limit() && did_spawn)
            did_spawn = false;
        
        if (did_spawn)
            spawn_timer += Time.deltaTime;
    }

    float get_spawn_time_limit() => base_spawn_delay;
    
    public void increaseSpawnedMonsters() => spawned_monsters += 1;

    public void decreaseSpawnedMonsters() => spawned_monsters -= 1;
}
