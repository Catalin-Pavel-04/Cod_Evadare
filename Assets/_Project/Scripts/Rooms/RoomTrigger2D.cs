using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoomTrigger2D : MonoBehaviour
{
    [SerializeField] private RoomController2D roomController;

    private bool loggedMissingRoomController;

    private void Awake()
    {
        if (roomController == null)
        {
            roomController = GetComponentInParent<RoomController2D>();
        }
    }

    private void Reset()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();

        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || !other.CompareTag("Player"))
        {
            return;
        }

        if (roomController == null)
        {
            LogMissingRoomController();
            return;
        }

        roomController.ActivateRoom();
    }

    private void LogMissingRoomController()
    {
        if (loggedMissingRoomController)
        {
            return;
        }

        Debug.LogWarning($"RoomTrigger2D on '{name}' has no RoomController2D assigned.", this);
        loggedMissingRoomController = true;
    }
}
