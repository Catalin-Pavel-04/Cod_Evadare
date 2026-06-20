using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RewardPickup2D : MonoBehaviour
{
    [SerializeField] private string rewardName = "Prototype Reward";

    private void Awake()
    {
        Collider2D pickupCollider = GetComponent<Collider2D>();

        if (pickupCollider != null)
        {
            pickupCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || !other.CompareTag("Player"))
        {
            return;
        }

        Debug.Log($"Player collected reward: {rewardName}.", this);
        Destroy(gameObject);
    }
}
