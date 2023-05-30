using UnityEngine;

namespace FirstPersonMobileTools.DynamicFirstPerson
{

    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(CameraLook))]
    public class nonMobileInput : MonoBehaviour {

        [HideInInspector] public float Sensitivity_X { private get { return _Sensitivity.x; } set { _Sensitivity.x = value * 50 / 3; }}
        [HideInInspector] public float Sensitivity_Y { private get { return _Sensitivity.y; } set { _Sensitivity.y = value * 50 / 3; }}

        [SerializeField] private KeyCode JumpInput;
        [SerializeField] private KeyCode SprintInput;
        [SerializeField] private KeyCode CrouchInput;
        [SerializeField] private bool LockCursor;
        [SerializeField] private Vector2 _Sensitivity = new Vector2(50f, 50f);

        private MovementController movementController;
        private CameraLook cameraLook;
        private Camera _camera;
        
        Quaternion y;
        Quaternion x;
        Vector2 delta = Vector2.zero;
        
        private void Start() {
            
            if (Camera.main != null)
                _camera = Camera.main;
            else Debug.LogError($"Can't find any main camera in scene!\n(Set your camera tag as MainCamera)", this);

            movementController = GetComponent<MovementController>();
            cameraLook = GetComponent<CameraLook>();
            
        }

        private void Update() {

        #if UNITY_EDITOR

            cameraLook.delta += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * _Sensitivity;

            movementController.External_Input_Movement = (Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up).normalized;

            if (Input.GetKeyDown(JumpInput))
                movementController.Input_Jump = true;
            if (Input.GetKeyUp(JumpInput))
                movementController.Input_Jump = false;

            if (Input.GetKeyDown(SprintInput))
                movementController.Input_Sprint = true;
            if (Input.GetKeyUp(SprintInput))
                movementController.Input_Sprint = false;

            if (Input.GetKeyDown(CrouchInput))
                movementController.Input_Crouch = true;
            if (Input.GetKeyUp(CrouchInput))
                movementController.Input_Crouch = false;

        #endif

        }

        void OnValidate()
        {

            if (LockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

    }

}