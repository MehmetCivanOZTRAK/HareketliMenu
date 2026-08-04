using UnityEngine;

public class MenuCharacterIK : MonoBehaviour
{
    private Animator anim;
    private Camera mainCam;

    [Header("Mouse Look")]
    [SerializeField] private float mouseDepth = 10f;
    [SerializeField] private Transform centerLookTarget;
    [SerializeField] private float lookFollowSpeed = 14f;

    [Header("Quit Fix")]
    [SerializeField] private float maxLookStepOnQuitExit = 0.1f;
    private bool justExitedQuit = false;

    [Header("Hover Lock")]
    [SerializeField] private float hoverLockDuration = 0.2f;
    private float hoverLockTimer = 0f;

    [Header("Look Clamp")]
    [SerializeField] private float minLookX = -0.7f;
    [SerializeField] private float maxLookX = 0.7f;
    [SerializeField] private float minLookY = 0.9f;
    [SerializeField] private float maxLookY = 2.1f;
    [SerializeField] private float minLookZ = 1.0f;
    [SerializeField] private float maxLookZ = 2.4f;

    [Header("Buttons")]
    [SerializeField] private Transform playTarget;
    [SerializeField] private Transform optionsTarget;

    [Header("Hand IK")]
    [SerializeField] private bool useLeftHand = true;
    [SerializeField] private float handMoveSpeed = 4f;
    [SerializeField] private float handWeightSpeed = 3f;
    [SerializeField] private float sideOffset = 0.02f;
    [SerializeField] private float heightOffset = -0.05f;
    [SerializeField] private float forwardOffset = 0.18f;
    [SerializeField] private float lookInfluenceStrength = 0.02f;

    [Header("Hand Clamp")]
    [SerializeField] private float minHandForward = 0.45f;
    [SerializeField] private float maxHandForward = 1.1f;
    [SerializeField] private float minHandHeight = 0.75f;
    [SerializeField] private float maxHandHeight = 1.5f;
    [SerializeField] private float minHandSideRight = 0.10f;
    [SerializeField] private float maxHandSideRight = 0.75f;
    [SerializeField] private float minHandSideLeft = -0.75f;
    [SerializeField] private float maxHandSideLeft = -0.05f;

    private Transform currentButtonTarget;
    private float currentHandWeight = 0f;
    private float targetHandWeight = 0f;
    private Vector3 currentHandPos;
    private Quaternion currentHandRot;
    private Vector3 currentLookPos;
    private bool isHoveringQuit = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        mainCam = Camera.main;

        currentHandPos = transform.position;
        currentHandRot = transform.rotation;

        if (centerLookTarget != null)
            currentLookPos = centerLookTarget.position;
        else
            currentLookPos = transform.position + transform.forward * 2f;
    }

    public void HoverPlay()
    {
        if (hoverLockTimer > 0f) return;

        currentButtonTarget = playTarget;
        targetHandWeight = 1f;
        isHoveringQuit = false;
    }

    public void HoverOptions()
    {
        if (hoverLockTimer > 0f) return;

        currentButtonTarget = optionsTarget;
        targetHandWeight = 1f;
        isHoveringQuit = false;
    }

    public void HoverQuit()
    {
        if (hoverLockTimer > 0f) return;

        currentButtonTarget = null;
        targetHandWeight = 0f;
        isHoveringQuit = true;
        justExitedQuit = false;
    }

    public void HoverNone()
    {
        if (isHoveringQuit)
            justExitedQuit = true;

        currentButtonTarget = null;
        targetHandWeight = 0f;
        isHoveringQuit = false;
    }

    public void ResetToNormal()
    {
        currentButtonTarget = null;
        targetHandWeight = 0f;
        isHoveringQuit = false;
        justExitedQuit = false;
    }

    public void LockHoverTemporarily()
    {
        hoverLockTimer = hoverLockDuration;
        ResetToNormal();
    }

    private void Update()
    {
        if (hoverLockTimer > 0f)
            hoverLockTimer -= Time.deltaTime;

        currentHandWeight = Mathf.Lerp(currentHandWeight, targetHandWeight, Time.deltaTime * handWeightSpeed);

        Vector3 targetLookPos;

        if (isHoveringQuit && centerLookTarget != null)
        {
            targetLookPos = centerLookTarget.position;
        }
        else
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = mouseDepth;
            Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);
            targetLookPos = ClampLookTarget(worldPos);

            if (justExitedQuit)
            {
                Vector3 delta = targetLookPos - currentLookPos;

                if (delta.magnitude > maxLookStepOnQuitExit)
                    targetLookPos = currentLookPos + delta.normalized * maxLookStepOnQuitExit;

                justExitedQuit = false;
            }
        }

        currentLookPos = Vector3.Lerp(currentLookPos, targetLookPos, Time.deltaTime * lookFollowSpeed);

        if (currentButtonTarget != null)
        {
            Vector3 lookDir = (currentLookPos - transform.position).normalized;

            Vector3 rawHandPos =
                currentButtonTarget.position
                + currentButtonTarget.right * sideOffset
                + currentButtonTarget.up * heightOffset
                + currentButtonTarget.forward * forwardOffset
                + lookDir * lookInfluenceStrength;

            Vector3 clampedHandPos = ClampHandTarget(rawHandPos);

            currentHandPos = Vector3.Lerp(currentHandPos, clampedHandPos, Time.deltaTime * handMoveSpeed);
            currentHandRot = Quaternion.Slerp(currentHandRot, currentButtonTarget.rotation, Time.deltaTime * handMoveSpeed);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (anim == null || mainCam == null) return;

        anim.SetLookAtWeight(0.9f, 0f, 0.8f, 0f, 0.65f);
        anim.SetLookAtPosition(currentLookPos);

        AvatarIKGoal handGoal = useLeftHand ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;

        anim.SetIKPositionWeight(handGoal, currentHandWeight);
        anim.SetIKRotationWeight(handGoal, 0.6f * currentHandWeight);

        if (currentHandWeight > 0.01f)
        {
            anim.SetIKPosition(handGoal, currentHandPos);
            anim.SetIKRotation(handGoal, currentHandRot);
        }
    }

    private Vector3 ClampLookTarget(Vector3 worldTarget)
    {
        Vector3 local = transform.InverseTransformPoint(worldTarget);

        local.x = Mathf.Clamp(local.x, minLookX, maxLookX);
        local.y = Mathf.Clamp(local.y, minLookY, maxLookY);
        local.z = Mathf.Clamp(local.z, minLookZ, maxLookZ);

        return transform.TransformPoint(local);
    }

    private Vector3 ClampHandTarget(Vector3 worldTarget)
    {
        Vector3 local = transform.InverseTransformPoint(worldTarget);

        local.z = Mathf.Clamp(local.z, minHandForward, maxHandForward);
        local.y = Mathf.Clamp(local.y, minHandHeight, maxHandHeight);

        if (useLeftHand)
            local.x = Mathf.Clamp(local.x, minHandSideLeft, maxHandSideLeft);
        else
            local.x = Mathf.Clamp(local.x, minHandSideRight, maxHandSideRight);

        return transform.TransformPoint(local);
    }
}