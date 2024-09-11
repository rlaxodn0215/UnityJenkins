using UnityEngine;

public class SliderPositionAutoSetter : MonoBehaviour
{
    [SerializeField]
    private Vector3 distance = Vector3.down * 20.0f;
    private Transform targetTransform;
    private RectTransform rectTransform;

    public void Setup(Transform target)
    {
        // Slider UI�� �Ѿƴٴ� Ÿ�� ����
        targetTransform = target;
        // RectTransform ������Ʈ ���� ������
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        // ���� ������ ����
        if(targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        // ������Ʈ ��ġ�� ���ŵ� ���� Slider UI�� �Բ� ��ġ�� �����ϱ� ���� LateUpdate���� ȣ��

        // ȭ�� ���� Ÿ�� ��ǥ + distance��ŭ ������ ��ġ�� Slider UI ����
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        rectTransform.position = screenPosition + distance;
    }
}

