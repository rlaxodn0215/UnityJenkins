using UnityEngine;
using UnityEngine.EventSystems;
public class ObjectDetector : MonoBehaviour
{
    [SerializeField]
    private TowerSpawner towerSpawner;
    [SerializeField]
    private TowerDataViewer towerDataViewer;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    // 마우스 팍킹으로 선택한 오브젝트 임시 저장
    private Transform hitTransform = null;

    private void Awake()
    {
        // main 카메라 할당
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // 마우스가 UI에 머물러 있을 때는 아래 코드가 실행되지 않도록 함
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // 마우스 왼쪽 버튼을 눌렀을 때
        if(Input.GetMouseButton(0))
        {
            // main 카메라에 ray 생성
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // 만약 충돌을 하면
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hitTransform = hit.transform;

                // 태그가 Tile이면
                if(hit.transform.CompareTag("Tile"))
                {
                    // 타워 소환
                    towerSpawner.SpawnTower(hit.transform);
                }
                // 타워를 선택하면 해당 타워 정보 출력
                else if(hit.transform.CompareTag("Tower"))
                {
                    towerDataViewer.OnPanel(hit.transform);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // 마우스를 눌렀을 때 선택한 오브젝트가 없거나 선택한 오브젝트가 타워가 아니면
            if(hitTransform == null || hitTransform.CompareTag("Tower") == false)
            {
                // 타워 정보 패널을 비활성화
                towerDataViewer.OffPanel();
            }

            hitTransform = null;
        }
    }
}
