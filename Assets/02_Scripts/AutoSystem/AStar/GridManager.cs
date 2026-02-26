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
        blocked = new bool[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 center = CellCenter(x, y);

                //샘플링하여 막힌 셀인지 확인, QueryTriggerInteraction.Ignore로 트리거 무시
                blocked[y, x] = Physics.CheckBox(center, halfExtents, Quaternion.identity, obstacleLayer, QueryTriggerInteraction.Ignore);
                //if (blocked[y, x]) Utils.DebugLog($"{x}, {y} 좌표는 막혀있다");
            }
        }
    }

    Vector3 CellCenter(int x, int y)
    {
        float worldX = origin.x + (x + 0.5f) * cellSize;
        float worldZ = origin.z + (y + 0.5f) * cellSize;

        return new Vector3(worldX, sampleY, worldZ);
    }

    //private void OnDrawGizmos()
    //{
    //    if(blocked == null)
    //    {
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawWireCube(origin + new Vector3(width * cellSize / 2f, sampleY, height * cellSize / 2f),
    //                                     new Vector3(width * cellSize, 0.1f, height * cellSize));
    //        return;
    //    }
    //
    //    for(int y = 0; y < height; y++)
    //    {
    //        for(int x = 0; x < width; x++)
    //        {
    //            Gizmos.color = blocked[y, x] ? Color.red : Color.green;
    //            Vector3 center = CellCenter(x, y);
    //            Gizmos.DrawCube(center, halfExtents * 2f);
    //            Gizmos.DrawWireCube(center, halfExtents * 2f);
    //        }
    //    }
    //}
}
