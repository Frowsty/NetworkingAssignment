using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Instance { get; private set; }
    
    private Vector3 target_direction;
    private Vector3 target_pos;
    private float interp_velocity;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        target_pos = transform.position;
    }

    public void MoveCamera(Vector3 target)
    {
        Vector3 pos = transform.position;
        pos.z = target.z;
        target_direction = (target - pos);
        
        interp_velocity = target_direction.magnitude * 5f;
        target_pos = transform.position + ((target_direction.normalized * interp_velocity) * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, target_pos, 1f);
    }
}
