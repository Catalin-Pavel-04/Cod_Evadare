using UnityEngine;

public class MinibossMarker2D : MonoBehaviour
{
    [SerializeField] private string minibossName = "Prototype Miniboss";

    public string MinibossName => string.IsNullOrWhiteSpace(minibossName) ? "Prototype Miniboss" : minibossName;
}
