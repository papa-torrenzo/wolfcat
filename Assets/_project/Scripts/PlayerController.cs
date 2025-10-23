using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;

namespace com.torrenzo.Foundation {
    public class PlayerController : ValidatedMonoBehaviour {
        [Header("References")]
        [SerializeField, Self] CharacterController characterController;
        [SerializeField, Self] Animator animator;
        [SerializeField, Anywhere] CinemachineCamera freeLookCam;
        [SerializeField, Anywhere] InputReader input;

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float rotationSpeed = 200f;
        [SerializeField] float smoothTime = 0.2f;
        [SerializeField] float playerHeight = 1.5f;

        private Vector3 moveToPosition;
        private bool isMoving;

        /*[Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float gravityMultiplier = 3f;*/



        const float ZeroF = 0f;

        Camera mainCam;
        Transform mainCamPos;

        float currentSpeed;
        float velocity;
        // float jumpVelocity;
        // float dashVelocity = 1f;

        Vector3 movement;

        // Animator parameters
        readonly int speedId = Animator.StringToHash("Speed");
        readonly float gapAllowance = 0.05f;

        void Awake() {

            mainCam = Camera.main;
            mainCamPos = mainCam.transform;
            freeLookCam.Follow = transform;
            freeLookCam.LookAt = transform;
            // Invoke event when observed transform is teleported, adjusting freeLookVCam's position accordingly
            freeLookCam.OnTargetObjectWarped(transform, transform.position - freeLookCam.transform.position - Vector3.forward);

        }

        void Start() => input.EnablePlayerActions();

        void OnEnable() {
            input.Jump += OnJump;
            //input.Move += OnMove;
        }

        void OnDisable() {
            input.Jump -= OnJump;
            //input.Move -= OnMove;

        }



        void OnJump(bool performed) {
           // noop
        }


        void Update() {
            // movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
            // HandleMovement();

            if (!isMoving) return;

            float distance = Vector3.Distance(moveToPosition, transform.position);

            if (distance > gapAllowance) {
                MovePlayerTo();
                UpdateAnimator();
            } else {
                isMoving = false;
            }
        }

        private void MovePlayerTo() {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, moveToPosition, step);
        }

        public void ClickToMove(Vector3 targetPosition) {
            moveToPosition = targetPosition + new Vector3(0f, playerHeight / 2f, 0f);
            isMoving = true;
        }

        void UpdateAnimator() {
            animator.SetFloat(speedId, currentSpeed);
        }

        public void HandleMovement() {
            // Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(mainCamPos.eulerAngles.y, Vector3.up) * movement;

            if (adjustedDirection.magnitude > ZeroF) {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            } else {
                SmoothSpeed(ZeroF);

                // Reset horizontal velocity for a snappy stop
                characterController.Move(Vector3.zero);
                //new Vector3(ZeroF, rb.linearVelocity.y, ZeroF);
            }
        }

        void HandleHorizontalMovement(Vector3 adjustedDirection) {
            // Move the player
            Vector3 velocity = adjustedDirection * (moveSpeed * Time.fixedDeltaTime);
            characterController.Move(velocity);
            Debug.Log(velocity);
        }

        void HandleRotation(Vector3 adjustedDirection) {
            // Adjust rotation to match movement direction
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        void SmoothSpeed(float value) {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }

    }
}