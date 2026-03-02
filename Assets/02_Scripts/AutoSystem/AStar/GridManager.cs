using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("그리드 정의")]
    public Vector3 origin = Vector3.zero; // 그리드 원점
    public float cellSize = 1f; // 셀 크기
    public int width = 10; // 그리드 너비
    public int height = 10; // 그리드 높이

    [Header("샘플링")]
    [SerializeField] LayerMask obstacleLayer; // 장애물 레이어
    [SerializeField] float sampleY = 0.5f; // 샘플링 높이
    [SerializeField] Vector3 halfExtents = new Vector3(0.45f, 1.0f, 0.45f); // 샘플링 반경

    public bool[,] blocked; // 그리드의 막힌 셀 정보

    private void Start()
    {
        GridData();
    }

    public void GridData()
    {
        blocked = new bool[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 center = GridToWorld(x, y);

                //샘플링하여 막힌 셀인지 확인, QueryTriggerInteraction.Ignore로 트리거 무시
                /*
                center: 스캔할 셀의 중심점으로 어디를 검사할지 결정하는 위치
                halfExtents: 박스 크기의 절반 값으로 중심에서 얼마나 뻗어 나갈지 정하는 함수
                Quaternion.identity: 박스를 회전시키지 않고 정방향으로 세우기위한 함수
                obstacleLayer: 무엇을 장애물로 볼것인지 판단하기위한 레이어
                QueryTriggerInteraction.Ignore: 트리거 체크가 된 투명한 콜라이더들은 장애물이 아니기때문에 무시하고 통과하기 위한 함수
                 */
                blocked[y, x] = Physics.CheckBox(center, halfExtents, Quaternion.identity, obstacleLayer, QueryTriggerInteraction.Ignore);
                //if (blocked[y, x]) Utils.DebugLog($"{x}, {y} 좌표는 막혀있다");
            }
        }
    }
    public Vector3 GridToWorld(int x, int y)
    {
        float worldX = origin.x + (x + 0.5f) * cellSize;
        float worldZ = origin.z + (y + 0.5f) * cellSize;

        return new Vector3(worldX, sampleY, worldZ);
    }

    //오버로딩
    public Vector3 GridToWorld(Vector2Int gridPos) => GridToWorld(gridPos.x, gridPos.y);

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.z - origin.z) / cellSize);
        return new Vector2Int(x, y);
    }


    private void OnDrawGizmos()
    {
        // 1. 기본 격자 틀 그리기 (실행 중이 아닐 때도 범위를 알 수 있게 함)
        Gizmos.color = Color.white;
        Vector3 gridCenter = origin + new Vector3(width * cellSize * 0.5f, sampleY, height * cellSize * 0.5f);
        Vector3 gridSize = new Vector3(width * cellSize, 0.01f, height * cellSize);
        Gizmos.DrawWireCube(gridCenter, gridSize);

        // 2. 셀 데이터 그리기
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 center = GridToWorld(x, y); // CellCenter 대신 기존 함수 사용

                //배열이 생성되지 않았을 때(에디터 모드)는 체크를 직접 수행해서 보여줌
                bool isBlocked;
                if (Application.isPlaying && blocked != null)
                {
                    isBlocked = blocked[y, x];
                }
                else
                {
                    //에디터 상에서도 실시간으로 어디가 막힐지 미리 보기 위함
                    isBlocked = Physics.CheckBox(center, halfExtents, Quaternion.identity, obstacleLayer, QueryTriggerInteraction.Ignore);
                }

                if (isBlocked)
                {
                    // 막힌 곳은 빨간색 반투명 박스
                    Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                    Gizmos.DrawCube(center, halfExtents * 2f);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(center, halfExtents * 2f);
                }
                else
                {
                    // 뚫린 곳은 녹색 선으로만 표시 (화면 가림 방지)
                    Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
                    Gizmos.DrawWireCube(center, new Vector3(cellSize, 0.01f, cellSize));
                }
            }
        }
    }
}
