using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintToolController : MonoBehaviour
{
    [SerializeField] private UltimateJoystick joystick;
    [SerializeField] private Transform modelHolder;
    [SerializeField] private Transform rolot;
    private Vector3 moveDirection = Vector3.zero;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector2 rotationLimit;
    [SerializeField] private float strafingSpeed;
    float angle = 0.0f;
    [SerializeField] private Transform distanceTarget;
    [SerializeField] private float maxDistance;
    [SerializeField] private Transform headPoint;
    [SerializeField] private Transform paintPosition;
    [SerializeField] private Transform HoldingPosition;
    [SerializeField] private Vector3 HoldingRotation;
    void Start()
    {
        
    }

    void Update()
    {
        if (distanceTarget == null) return;
        float horizontalInput = joystick.HorizontalAxis;
        float verticalInput = joystick.VerticalAxis;
        moveDirection.x = horizontalInput;
        moveDirection.z = verticalInput;
        moveDirection.Normalize();

        if (Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput))
            rolot.transform.localEulerAngles = Vector3.zero;
        else if (Mathf.Abs(verticalInput) < Mathf.Abs(horizontalInput))
            rolot.transform.localEulerAngles = new Vector3(0, 90, 45);

        RotatePaintingTool();
    }

    private void RotatePaintingTool()
    {
        angle = rotationLimit.y;
        modelHolder.transform.rotation = Quaternion.Euler(angle, 0.0f, 0.0f);
        bool result = false;
        while (!result && distanceTarget != null)
        {
            if (Mathf.Abs(headPoint.position.z - distanceTarget.position.z) > 0.2f)
            {
                angle -= 0.01f;
                modelHolder.transform.rotation = Quaternion.Euler(angle, 0.0f, 0.0f);
            }
            else
            {
                result = true;
                break;
            }
            if (angle < rotationLimit.x)
            {
                angle = rotationLimit.y;
                modelHolder.transform.rotation = Quaternion.Euler(angle, 0.0f, 0.0f);
                break;
            }
        }
    }

    public void SetDistanceTarget(Transform target)
    {
        modelHolder.transform.position = paintPosition.position;
        StartCoroutine(SetTargetRoutine(target));
    }

    IEnumerator SetTargetRoutine(Transform target)
    {
        yield return null;
        distanceTarget = target;
    }

    public void ResetTool()
    {
        distanceTarget = null;
        modelHolder.transform.position = HoldingPosition.position;
        modelHolder.transform.localRotation = Quaternion.Euler(HoldingRotation);
    }
}
