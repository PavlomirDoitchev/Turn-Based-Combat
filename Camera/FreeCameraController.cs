using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeCameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float fastMoveMultiplier = 2f;
    [SerializeField] private float drag = 10f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 90f;
    [Range(-89f, 89f)] public float minVerticalAngle = -45f;
    [Range(-89f, 89f)] public float maxVerticalAngle = 45f;

    [Header("Zoom")]
    [SerializeField] private float distance = 8f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 20f;
    private float zoomVelocity;

    [Header("Obstruction")]
    [SerializeField] private LayerMask obstructionMask = -1;

    private Camera cam;
    private Transform camTransform;

    private Vector2 orbitAngles = new Vector2(30f, 0f);
    private Vector3 focusPoint;
    private Vector3 velocity;

    private Vector3 CameraHalfExtents
    {
        get
        {
            Vector3 halfExt = Vector3.zero;
            halfExt.y = cam.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * cam.fieldOfView);
            halfExt.x = halfExt.y * cam.aspect;
            return halfExt;
        }
    }

    private void Awake()
    {
        cam = GetComponent<Camera>();
        camTransform = transform;
        focusPoint = camTransform.position + camTransform.forward * distance;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
        UpdateCameraPosition();
    }

    private void HandleMovement()
    {

        Vector3 forward = camTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = camTransform.right;
        right.y = 0;
        right.Normalize();

        Vector3 input = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) input += forward;
        if (Input.GetKey(KeyCode.S)) input -= forward;
        if (Input.GetKey(KeyCode.A)) input -= right;
        if (Input.GetKey(KeyCode.D)) input += right;
        if (Input.GetKey(KeyCode.Q)) input -= Vector3.up;
        if (Input.GetKey(KeyCode.E)) input += Vector3.up;

        if (input != Vector3.zero)
        {
            float speed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift)) speed *= fastMoveMultiplier;

            velocity += input.normalized * speed * Time.deltaTime;
        }

        velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * drag);
        focusPoint += velocity * Time.deltaTime;
    }

    private void HandleRotation()
    {
        if (!Input.GetMouseButton(2 ))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector2 mouse = new Vector2(
            -Input.GetAxis("Mouse Y"),
            Input.GetAxis("Mouse X")
        );

        orbitAngles += mouse * rotationSpeed * Time.unscaledDeltaTime;

        orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);
        if (orbitAngles.y < 0) orbitAngles.y += 360;
        if (orbitAngles.y >= 360) orbitAngles.y -= 360;
    }

    private void HandleZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            float target = Mathf.Clamp(distance - Input.mouseScrollDelta.y * 100, minDistance, maxDistance);
            distance = Mathf.SmoothDamp(distance, target, ref zoomVelocity, 0.2f);
        }
    }

    private void UpdateCameraPosition()
    {
        Quaternion lookRot = Quaternion.Euler(orbitAngles);

        Vector3 desiredCamPos = focusPoint - lookRot * Vector3.forward * distance;
        Vector3 rectOffset = lookRot * (Vector3.forward * cam.nearClipPlane);
        Vector3 rectPos = desiredCamPos + rectOffset;

        Vector3 castFrom = focusPoint;
        Vector3 castLine = rectPos - castFrom;
        float castDist = castLine.magnitude;
        Vector3 castDir = castLine / castDist;

        if (Physics.BoxCast(castFrom, CameraHalfExtents, castDir, out RaycastHit hit,
            lookRot, castDist, obstructionMask))
        {
            rectPos = castFrom + castDir * hit.distance;
            desiredCamPos = rectPos - rectOffset;
        }

        // Apply camera shake
        Vector3 posShake = ImpulseManager.GetPositionShake();
        Vector3 rotShake = ImpulseManager.GetRotationShake();

        camTransform.position = desiredCamPos + posShake;
        camTransform.rotation = lookRot * Quaternion.Euler(rotShake);
    }
}
