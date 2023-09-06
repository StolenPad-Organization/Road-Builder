using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolAngleController : MonoBehaviour
{
    [SerializeField] private UltimateJoystick joystick;
    [SerializeField] private Transform modelHolder;
    public Transform toolHead;
    private Vector3 moveDirection = Vector3.zero;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector2 rotationLimit;
    [SerializeField] private float strafingSpeed;
    float angle = 0.0f;
    [SerializeField] private Transform distanceTarget;
    [SerializeField] private float maxDistance;
    [SerializeField] private Transform headPoint;
    [SerializeField] private Transform usePosition;
    [SerializeField] private Transform HoldingPosition;
    [SerializeField] private Vector3 HoldingRotation;
    [SerializeField] private bool canRotate;

    [Header("Showing Fake & Real Models")]
    [SerializeField] private GameObject fakeModel;
    [SerializeField] private Transform fakeModelPos;
    [SerializeField] private GameObject realModel;

    [Header("Scale and Upgrade")]
    [SerializeField] private Transform headPos;
    [SerializeField] private Transform toolHandle;
    [SerializeField] private float toolLength;
    [SerializeField] private float toolWidth;
    [SerializeField] private int index;
    [SerializeField] private Vector2 lengthLimits;
    [SerializeField] private float lengthIncrease;
    [SerializeField] private Vector2 widthLimits;
    [SerializeField] private float widthIncrease;
    [SerializeField] private Transform otherWidthObject;

    void Start()
    {
        joystick = PlayerController.instance.movementController.joystick;
        //CalculateScale(index);
    }

    void Update()
    {
        if (distanceTarget == null) return;
        float horizontalInput = joystick.HorizontalAxis;
        float verticalInput = joystick.VerticalAxis;
        moveDirection.x = horizontalInput;
        moveDirection.y = verticalInput;
        moveDirection.Normalize();

        //if (Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput))
        //    toolHead.transform.localEulerAngles = Vector3.zero;
        //else if (Mathf.Abs(verticalInput) < Mathf.Abs(horizontalInput))
        //    toolHead.transform.localEulerAngles = new Vector3(0, 90, 45);

        RotatePaintingTool();

        if (canRotate)
        {
            toolHead.transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.forward);
        }
        else
        {
            toolHead.transform.localEulerAngles = Vector3.zero;
        }
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
        modelHolder.transform.position = usePosition.position;
        StartCoroutine(SetTargetRoutine(target));

        fakeModel.SetActive(false);
        realModel.SetActive(true);
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
        toolHead.transform.eulerAngles = Vector3.zero;

        fakeModel.SetActive(true);
        realModel.SetActive(false);
    }

    public void CalculateLength(int _index)
    {
        index = _index;
        toolLength = lengthLimits.x + (lengthIncrease * index);
        toolLength = Mathf.Clamp(toolLength, lengthLimits.x, lengthLimits.y);
        Vector3 toolScale = toolHandle.localScale;
        toolScale.y = toolLength;
        toolHandle.localScale = toolScale;
        toolHead.position = headPos.position;
    }

    public void CalculateWidth(int _index)
    {
        index = _index;
        toolWidth = widthLimits.x + (widthIncrease * index);
        toolWidth = Mathf.Clamp(toolWidth, widthLimits.x, widthLimits.y);
        Vector3 toolScale = toolHead.localScale;
        toolScale.x = toolWidth;
        toolHead.localScale = toolScale;
        if(otherWidthObject != null)
        {
            toolScale = otherWidthObject.localScale;
            toolScale.x = toolWidth * 2;
            otherWidthObject.localScale = toolScale;
        }
    }

    public void OnPick()
    {
        fakeModel.transform.SetParent(fakeModelPos);
        fakeModel.transform.localPosition = Vector3.zero;
        fakeModel.transform.localEulerAngles = Vector3.zero;
        fakeModel.transform.localScale = Vector3.one;
    }
}
