using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientPlayerMove : NetworkBehaviour
{
    [SerializeField]
    private CharacterController m_CharacterController;
    [SerializeField]
    private PlayerInput m_PlayerInput;
    [SerializeField]
    private Transform m_CameraFollow;
    
    private float m_MovementSpeed = 5f;
    private Vector2 direction = Vector2.zero;

    void Awake()
    {
        m_CharacterController.enabled = false;
        m_PlayerInput.enabled = false;
        m_CameraFollow = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsClient;
        
        if (!IsOwner)
        {
            enabled = false;
            m_CharacterController.enabled = true;
            m_PlayerInput.enabled = false;

            return;
        }

        m_CharacterController.enabled = true;
        m_PlayerInput.enabled = true;
    }
    
    public void OnMove(InputValue value) => direction = value.Get<Vector2>();

    private void Update()
    {
        CameraMovement.Instance.MoveCamera(transform.position);
        
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = targetPos - transform.position;
        float lookAngle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, lookAngle);
        
        if (m_CharacterController.enabled)
            m_CharacterController.Move(Time.deltaTime * m_MovementSpeed * direction );
    }
}