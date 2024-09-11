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
    // ���콺 ��ŷ���� ������ ������Ʈ �ӽ� ����
    private Transform hitTransform = null;

    private void Awake()
    {
        // main ī�޶� �Ҵ�
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // ���콺�� UI�� �ӹ��� ���� ���� �Ʒ� �ڵ尡 ������� �ʵ��� ��
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // ���콺 ���� ��ư�� ������ ��
        if(Input.GetMouseButton(0))
        {
            // main ī�޶� ray ����
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // ���� �浹�� �ϸ�
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hitTransform = hit.transform;

                // �±װ� Tile�̸�
                if(hit.transform.CompareTag("Tile"))
                {
                    // Ÿ�� ��ȯ
                    towerSpawner.SpawnTower(hit.transform);
                }
                // Ÿ���� �����ϸ� �ش� Ÿ�� ���� ���
                else if(hit.transform.CompareTag("Tower"))
                {
                    towerDataViewer.OnPanel(hit.transform);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // ���콺�� ������ �� ������ ������Ʈ�� ���ų� ������ ������Ʈ�� Ÿ���� �ƴϸ�
            if(hitTransform == null || hitTransform.CompareTag("Tower") == false)
            {
                // Ÿ�� ���� �г��� ��Ȱ��ȭ
                towerDataViewer.OffPanel();
            }

            hitTransform = null;
        }
    }
}
