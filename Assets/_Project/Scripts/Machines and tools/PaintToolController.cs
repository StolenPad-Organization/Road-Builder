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
        //moveDirection.Normalize();
        //if (moveDirection.z != 0)
        //{
        //rolot.transform.localEulerAngles = Vector3.zero;
        //rotate modelHolder and adjust the z position of the player
        //angle += rotationSpeed * -moveDirection.z * Time.deltaTime;
        //angle = Mathf.Clamp(angle, rotationLimit.x, rotationLimit.y);

        //float distance = distanceTarget.position.z - transform.position.z;
        //Debug.Log(distance);
        //float t = Mathf.InverseLerp(0.0f, maxDistance, distance);
        //angle = Mathf.Lerp(rotationLimit.x, rotationLimit.y, t);
        //modelHolder.transform.localRotation = Quaternion.Euler(angle, 0.0f, 0.0f);

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
        //}
    }

    private void Strafe()
    {
        moveDirection.Normalize();
        if (moveDirection.x != 0)
        {
            //move the player on it's x axis (pos + transform.right)
            //rolot.transform.localEulerAngles = Vector3.up * 90;
        }
    }

    public void SetDistanceTarget(Transform target)
    {
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
        modelHolder.transform.localRotation = Quaternion.Euler(rotationLimit.y, 0.0f, 0.0f);
    }
}
