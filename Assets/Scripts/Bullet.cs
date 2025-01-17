using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject damage_prefab;
    
    private Weapon weapon;
    private Rigidbody2D rb;
    
    private float bullet_speed = 6.0f;
    private float travel_time;

    private Vector3 target_pos;

    private ObjectPool<Bullet> bullet_pool;

    public bool is_disabled = false;
    public Player player;

    private void Awake()
    {
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player == null && p.GetComponent<NetworkBehaviour>().IsOwner)
                player = p.GetComponent<Player>();
        }

        weapon = player.GetComponentInChildren<Weapon>();   
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (travel_time > 1.5)
        {
            bullet_pool.Release(this);
            travel_time = 0;
            return;
        }

        rb.linearVelocity = transform.right * bullet_speed;
        
        travel_time += Time.deltaTime;
    }
    
    public void setPool(ObjectPool<Bullet> pool) => bullet_pool = pool;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!player.IsHost) return;
        
        collision.TryGetComponent<Monster>(out Monster monster);
        if (monster != null && !is_disabled)
        {
            if (collision.gameObject.activeSelf)
            {
                int damage = weapon.getDamage();
                monster.takeDamage(damage);
            }

            // don't release bullet upon trigger since we want it to pierce enemies
            is_disabled = true;
            travel_time = 0;
        }
    }
}
