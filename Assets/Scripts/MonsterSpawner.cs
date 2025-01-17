using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class MonsterSpawner : NetworkBehaviour
{
    float base_spawn_delay = 10f;
    
    [SerializeField]
    private Monster monster_prefab;
    
    private Dictionary<int, Monster> monsters = new Dictionary<int, Monster>();
    
    private ObjectPool<Monster> monster_pool;
    
    private int max_spawn = 100;
    private int spawned_monsters = 0;
    private float spawn_timer = 0f;
    private bool did_spawn = false;

    private static int monsterID = 1;

    public override void OnNetworkSpawn()
    {
        monster_pool = new ObjectPool<Monster>(createMonster, onTakeMonster, onReturnMonster, onDestroyMonster,
            true, 200, 1500);
    }
    void Update()
    {
        if (!IsHost) return;
        
        if (NetworkManager.Singleton.ConnectedClients.Count > 1) 
            SpawnMonsters();
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
        if (monster.myNetworkObject != null)
            monster.myNetworkObject.Spawn();
        else
            monster.GetComponent<NetworkObject>().Spawn();
    }

    private void onReturnMonster(Monster monster)
    {
        monster.gameObject.SetActive(false);
        DecreaseSpawnedMonsters();
    }

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

    [ServerRpc]
    private void SpawnMonsterServerRpc()
    {
        var m = monster_pool.Get();
        m.monsterID = monsterID;
        //SpawnMonsterClientRpc(new Vector4(m.transform.position.x, m.transform.position.y, m.transform.position.z, monsterID));
        IncreaseSpawnedMonsters();
        monsterID++;
    }
    
    private void SpawnMonsters()
    {
        if (spawned_monsters < max_spawn && !did_spawn)
        {
            SpawnMonsterServerRpc();
            spawn_timer = 0f;
        }
        else
            did_spawn = true;

        if (spawn_timer >= GetSpawnTimeLimit() && did_spawn)
            did_spawn = false;
        
        if (did_spawn)
            spawn_timer += Time.deltaTime;
    }

    float GetSpawnTimeLimit() => base_spawn_delay;
    
    public void IncreaseSpawnedMonsters() => spawned_monsters += 1;

    public void DecreaseSpawnedMonsters() => spawned_monsters -= 1;
}
