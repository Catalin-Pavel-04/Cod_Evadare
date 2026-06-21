using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ObjectiveTrigger2D : MonoBehaviour
{
    [SerializeField] private ObjectiveUI2D objectiveUI;
    [SerializeField] private string objectiveMessage;
    [SerializeField] private string hintMessage;
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered;

    private void Awake()
    {
        ConfigureCollider();
    }

    private void Reset()
    {
        ConfigureCollider();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && triggerOnce)
        {
            return;
        }

        if (other == null || !other.CompareTag("Player"))
        {
            return;
        }

        if (objectiveUI == null)
        {
            objectiveUI = FindObjectOfType<ObjectiveUI2D>();
        }

        if (objectiveUI == null)
        {
            Debug.LogWarning($"{name} cannot update objective because no ObjectiveUI2D was found.", this);
            return;
        }

        if (!string.IsNullOrWhiteSpace(objectiveMessage))
        {
            objectiveUI.SetObjective(objectiveMessage);
        }

        if (!string.IsNullOrWhiteSpace(hintMessage))
        {
            objectiveUI.SetHint(hintMessage);
        }

        hasTriggered = true;
    }

    private void ConfigureCollider()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();

        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }
}
