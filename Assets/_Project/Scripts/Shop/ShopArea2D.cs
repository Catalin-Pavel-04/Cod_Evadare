using UnityEngine;

public class ShopArea2D : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            Debug.Log("Player entered the shop area.", this);
        }
    }
}
