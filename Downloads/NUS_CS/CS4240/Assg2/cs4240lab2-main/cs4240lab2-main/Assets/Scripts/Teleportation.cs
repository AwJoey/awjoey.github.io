using UnityEngine;
using UnityEngine.InputSystem;

public class Teleportation : MonoBehaviour
{
    public GameObject Player;
    public InputActionReference selectAction;
    public InputActionReference teleportAction;
    public GameObject teleportIndicatorPrefab;
    public LayerMask teleportLayerMask;

    private GameObject teleportIndicator;
    private Vector3 teleportPosition;
    private bool isSelecting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectAction.action.Enable();
        teleportAction.action.Enable();

        teleportIndicator = Instantiate(teleportIndicatorPrefab);
        teleportIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (selectAction.action.ReadValue<float>() > 0.1f && !isSelecting)
        {
            isSelecting = true;
            teleportIndicator.SetActive(true);
        }
        else if (selectAction.action.ReadValue<float>() <= 0.1f && isSelecting)
        {
            isSelecting = false;
            teleportIndicator.SetActive(false);
        }

        if (isSelecting)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 100f, teleportLayerMask))
            {
                teleportPosition = hit.point;
                teleportIndicator.transform.position = teleportPosition;
            }
        }

        if (teleportAction.action.triggered && isSelecting)
        {
            TeleportToTarget();
        }
    }

    private void TeleportToTarget()
    {

        if (Player != null)
        {
            // Can be changed if our place has hills or something whith slope.
            // value for y is fixed right now
            Vector3 newPosition = new Vector3(teleportPosition.x, Player.transform.position.y, teleportPosition.z);
            Player.transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("XR Origin reference not set in Inspector!");
        }
        teleportIndicator.SetActive(false);
    }
}
