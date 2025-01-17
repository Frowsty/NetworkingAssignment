using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    [SerializeField] Bullet bullet_prefab;
    [SerializeField] Player player;
    
    private ObjectPool<Bullet> bullet_pool;
    private GameObject[] powerup_info;

    private Vector3 direction;
    
    private float time_since_shot = 0;
    private float fire_rate = 0.15f;
    
    // used for bullet z rotation offset
    private Vector3 rot;
 
    private Bullet createBullet()
    {
        // create new instance
        Bullet bullet = Instantiate(bullet_prefab, transform.position, transform.rotation);
        
        bullet.setPool(bullet_pool);
        return bullet;
    }

    private void onTakeBullet(Bullet bullet)
    {
        bullet.transform.position = new Vector3(transform.position.x, transform.position.y, -3); // -3 since bullets should be ontop of everything
        bullet.transform.rotation = transform.rotation;
        bullet.is_disabled = false;
        
        bullet.gameObject.SetActive(true);
    }

    private void onReturnBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void onDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }
    
    void Start()
    {
        bullet_pool = new ObjectPool<Bullet>(createBullet, onTakeBullet, onReturnBullet, onDestroyBullet, true, 1000, 3000);
        time_since_shot = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.time - time_since_shot) >= fire_rate)
        {
            bullet_pool.Get();
            time_since_shot = Time.time;
        }
    }
    
    public int getDamage()
    {
        return 100;
    }
}
