using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController m_CharacterController;
    private float m_MovementSpeed = 10f;
    
    private Vector2 direction = Vector2.zero;
    
    public void OnMove(InputValue value) => direction = value.Get<Vector2>();

    private void Update()
    {
        if (m_CharacterController.enabled)
            m_CharacterController.Move(m_MovementSpeed * Time.deltaTime * direction);
    }
}
