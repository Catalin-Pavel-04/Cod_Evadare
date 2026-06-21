using UnityEngine;

public class PickupBob2D : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.12f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float rotateSpeed;

    private Vector3 initialLocalPosition;
    private float timeOffset;

    private void Start()
    {
        initialLocalPosition = transform.localPosition;
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * bobSpeed + timeOffset) * bobHeight;
        transform.localPosition = initialLocalPosition + new Vector3(0f, yOffset, 0f);

        if (rotateSpeed > 0f)
        {
            transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        }
    }

    private void OnValidate()
    {
        bobHeight = Mathf.Max(0f, bobHeight);
        bobSpeed = Mathf.Max(0f, bobSpeed);
        rotateSpeed = Mathf.Max(0f, rotateSpeed);
    }
}
