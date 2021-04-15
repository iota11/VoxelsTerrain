using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [Range(1.0f, 10.0f)]
        [SerializeField]
        private float m_movementSpeed = 5.0f;
        [Range(1.0f, 10.0f)]
        [SerializeField]
        private float m_jumpHeight = 1.5f;

        [Header("Look")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float m_lookSensitivity = 0.1f;
        [SerializeField]
        private Camera m_camera;

        private float Gravity => Physics.gravity.y;

        private Vector2 m_moveDelta;
        private Vector2 m_lookDelta;
        private bool m_wantsToJump;

        private Vector2 m_rotation;
        private float m_velocityY;

        private CharacterController m_controller;

        public void OnMove(InputAction.CallbackContext context) => m_moveDelta = context.ReadValue<Vector2>();

        public void OnJump(InputAction.CallbackContext context) => m_wantsToJump = context.ReadValueAsButton();

        public void OnLook(InputAction.CallbackContext context) => m_lookDelta += context.ReadValue<Vector2>();

        private void Start()
        {
            m_controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void ApplyLook()
        {
            m_rotation.y += m_lookDelta.x * m_lookSensitivity;
            m_rotation.x -= m_lookDelta.y * m_lookSensitivity;
            m_rotation.x = Mathf.Clamp(m_rotation.x, -90.0f, 90.0f);

            m_camera.transform.localRotation = Quaternion.Euler(m_rotation.x, 0.0f, 0.0f);
            transform.localRotation = Quaternion.Euler(0.0f, m_rotation.y, 0.0f);

            m_lookDelta = Vector2.zero;
        }

        private void Jump()
        {
            m_velocityY = Mathf.Sqrt(-2.0f * Gravity * m_jumpHeight);
        }

        private void ApplyMovement()
        {
            if (m_wantsToJump && m_controller.isGrounded)
            {
                Jump();
            }

            m_velocityY += Gravity * Time.deltaTime;

            Vector3 horizontalVelocity = (transform.right * m_moveDelta.x + transform.forward * m_moveDelta.y) * m_movementSpeed;
            Vector3 verticalVelocity = Vector3.up * m_velocityY;

            m_controller.Move((horizontalVelocity + verticalVelocity) * Time.deltaTime);

            if (m_controller.isGrounded)
            {
                m_velocityY = 0.0f;
            }
        }

        private void Update()
        {
            ApplyLook();
            ApplyMovement();
        }
    }
}