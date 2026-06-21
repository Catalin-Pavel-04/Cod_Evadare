using UnityEngine;

public class BossMarker2D : MonoBehaviour
{
    [SerializeField] private string bossName = "Experiment-01";

    public string BossName => string.IsNullOrWhiteSpace(bossName) ? "Experiment-01" : bossName;
}
