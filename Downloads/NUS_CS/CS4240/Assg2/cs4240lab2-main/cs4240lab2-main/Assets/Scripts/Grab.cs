using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.OpenXR.Input;

public class Grab : MonoBehaviour
{
    public InputActionReference grabAction;
    public InputActionReference fireAction;

    public InputActionProperty haptic;
    public InputActionProperty controllerVelocity;
    public InputActionProperty controllerAngularVelocity;

    public float grabRadius = 1.0f;
    public float shootForce = 10.0f;
    public LayerMask grabMask;

    private bool grabbing = false;
    private GameObject grabbedObject;
    private Rigidbody grabbedRb;


    void Start()
    {
        if (grabAction == null || haptic == null)
            return;

        grabAction.action.Enable();
        fireAction.action.Enable();
        haptic.action.Enable();

        grabAction.action.performed += OnGrabPerformed;
        fireAction.action.performed += OnFirePerformed;
        grabAction.action.canceled += OnGrabCanceled;

    }

    private void Update()
    {

    }

    void OnGrabPerformed(InputAction.CallbackContext ctx)
    {
        //var control = grabAction.action.activeControl;
        //if (null == control)
        //    return;
        GrabObject();

    }

    void OnGrabCanceled(InputAction.CallbackContext ctx)
    {
        DropObject();
    }

    void OnFirePerformed(InputAction.CallbackContext ctx)
    {
        if (grabbing)
        {
            ShootObject();
        }
    }

    void GrabObject()
    {
        grabbing = true;
        Collider[] colliders = Physics.OverlapSphere(transform.position, grabRadius, grabMask);

        if (colliders.Length > 0)
        {
            GameObject nearestObject = null;
            float minDistance = float.MaxValue;
            Rigidbody nearestRb = null;

            foreach (Collider col in colliders)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestObject = col.gameObject;
                    nearestRb = nearestObject.GetComponent<Rigidbody>();
                }
            }

            if (nearestObject != null && nearestRb != null)
            {
                grabbedObject = nearestObject;
                grabbedRb = nearestRb;
                grabbedRb.isKinematic = true;
                grabbedRb.useGravity = false;

                grabbedObject.transform.SetParent(transform);
                grabbedObject.transform.localPosition = Vector3.zero;
                grabbedObject.transform.localRotation = Quaternion.identity;
            }
        }
    }


    void DropObject()
    {
        grabbing = false;
        if (grabbedObject != null)
        {
            grabbedObject.transform.parent = null;
            if (grabbedRb != null)
            {
                grabbedRb.isKinematic = false;
                grabbedRb.useGravity = true;

                if (controllerVelocity != null && controllerVelocity.action != null)
                    grabbedRb.linearVelocity = controllerVelocity.action.ReadValue<Vector3>();

                if (controllerAngularVelocity != null && controllerAngularVelocity.action != null)
                    grabbedRb.angularVelocity = controllerAngularVelocity.action.ReadValue<Vector3>();
            }

            grabbedObject = null;
            grabbedRb = null;
        }
    }

    void ShootObject()
    {
        grabbing = false;
        if (grabbedObject != null)
        {
            grabbedObject.transform.parent = null;
            if (grabbedRb != null)
            {
                grabbedRb.isKinematic = false;
                grabbedRb.useGravity = true;

                Vector3 shootDirection = transform.forward;

                if (controllerVelocity != null && controllerVelocity.action != null)
                    grabbedRb.linearVelocity = controllerVelocity.action.ReadValue<Vector3>() + shootDirection * shootForce;
                else
                    grabbedRb.linearVelocity = shootDirection * shootForce;

                if (controllerAngularVelocity != null && controllerAngularVelocity.action != null)
                    grabbedRb.angularVelocity = controllerAngularVelocity.action.ReadValue<Vector3>();
            }

            grabbedObject = null;
            grabbedRb = null;
        }
    }

}
