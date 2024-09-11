using UnityEngine;

public class SliderPositionAutoSetter : MonoBehaviour
{
    [SerializeField]
    private Vector3 distance = Vector3.down * 20.0f;
    private Transform targetTransform;
    private RectTransform rectTransform;

    public void Setup(Transform target)
    {
        // Slider UI가 쫓아다닐 타겟 설정
        targetTransform = target;
        // RectTransform 컴포넌트 정보 얻어오기
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        // 적이 죽으면 삭제
        if(targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        // 오브젝트 위치가 갱신된 이후 Slider UI도 함께 위치를 설정하기 위해 LateUpdate에서 호출

        // 화면 내의 타겟 좌표 + distance만큼 떨어진 위치에 Slider UI 놓기
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        rectTransform.position = screenPosition + distance;
    }
}

