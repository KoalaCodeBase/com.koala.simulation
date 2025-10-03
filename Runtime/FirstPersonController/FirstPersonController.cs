using UnityEngine;

namespace Koala.Simulation.FirstPersonController
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Movement")]
        public float MoveSpeed = 4f;
        public float SprintSpeed = 6f;
        public float RotationSpeed = 1f;
        public float SpeedChangeRate = 10f;

        [Header("Jump & Gravity")]
        public float JumpHeight = 1.2f;
        public float Gravity = -15f;
        public float JumpTimeout = 0.1f;
        public float FallTimeout = 0.15f;

        [Header("Ground Check")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public LayerMask GroundLayers;

        [Header("Camera")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 90f;
        public float BottomClamp = -90f;

        private CharacterController _controller;
        private PlayerInputState _input;
        private GameObject _mainCamera;

        private float _cinemachinePitch;
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private const float _threshold = 0.01f;

        private void Awake()
        {
            _mainCamera = Camera.main?.gameObject;
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputState>();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            GroundedCheck();
            JumpAndGravity();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void GroundedCheck()
        {
            Vector3 pos = transform.position;
            Vector3 spherePos = new Vector3(pos.x, pos.y - GroundedOffset, pos.z);
            Grounded = Physics.CheckSphere(spherePos, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude < _threshold)
                return;

            float delta = 1f;

            _cinemachinePitch += _input.look.y * RotationSpeed * delta;
            _rotationVelocity = _input.look.x * RotationSpeed * delta;

            _cinemachinePitch = ClampAngle(_cinemachinePitch, BottomClamp, TopClamp);
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachinePitch, 0f, 0f);

            transform.Rotate(Vector3.up * _rotationVelocity);
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;

            if (Mathf.Abs(currentHorizontalSpeed - targetSpeed) > speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 inputDir = new Vector3(_input.move.x, 0f, _input.move.y).normalized;
            if (_input.move != Vector2.zero)
                inputDir = transform.right * _input.move.x + transform.forward * _input.move.y;

            Vector3 velocity = inputDir.normalized * (_speed * Time.deltaTime) + Vector3.up * _verticalVelocity * Time.deltaTime;
            _controller.Move(velocity);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_verticalVelocity < 0f)
                    _verticalVelocity = -2f;

                if (_input.jump && _jumpTimeoutDelta <= 0f)
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                if (_jumpTimeoutDelta > 0f)
                    _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta > 0f)
                    _fallTimeoutDelta -= Time.deltaTime;

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
                _verticalVelocity += Gravity * Time.deltaTime;
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) angle += 360f;
            if (angle > 360f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }

        private void OnDrawGizmosSelected()
        {
            Color color = Grounded ? new Color(0f, 1f, 0f, 0.35f) : new Color(1f, 0f, 0f, 0.35f);
            Gizmos.color = color;

            Vector3 pos = transform.position;
            Gizmos.DrawSphere(new Vector3(pos.x, pos.y - GroundedOffset, pos.z), GroundedRadius);
        }
    }
}