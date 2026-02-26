using System.Collections.Generic;
using UnityEngine;

public class AStarFindPath : MonoBehaviour
{
    [Header("세팅")]
    [SerializeField] GridManager gridManager; //그리드 매니저 참조
    public LayerMask obstacleLayer; //장애물 레이어
    public GameObject floorSample; //바닥 오브젝트 샘플
    public float cellSize; //셀 크기
    public float checkSize = 0.35f; //충돌 체크 크기

    private void Awake()
    {
        if (floorSample != null)
        {
            cellSize = floorSample.GetComponent<Renderer>().bounds.size.x;
        }
    }

    /// <summary>
    /// 월드 좌표를 그리드 좌표로 변환하는 함수
    /// (x,z) 사용 3D 공간에서 y는 고도이므로 z를 사용하여 그리드 좌표로 변환
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public Vector2Int WorldToGrid(Vector3 worldPos) => gridManager.WorldToGrid(worldPos);
    

    /// <summary>
    /// 그리드 좌표를 월드 좌표로 변환하는 함수    
    /// /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    public Vector3 GridToWorld(Vector2Int gridPos) => gridManager.GridToWorld(gridPos);
    

    /// <summary>
    /// A* 알고리즘을 사용하여 시작점에서 목표점까지의 최적 경로를 찾는 함수
    /// start와 end는 그리드 좌표로 입력
    /// 반환: 경로가 존재하면 그리드 좌표의 리스트, 경로가 없으면 null
    /// 대각선 이동을 허용하며, 이동 비용은 직선 이동이 10, 대각선 이동이 14로 계산
    /// 1과 1.41 대신 10과 14를 사용하여 정수 계산으로 성능 향상
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public List<Vector2Int> GetAstarPath(Vector2Int start, Vector2Int end)
    {
        //아직 평가할 후보 노드들
        var openSet = new PriorityQueue<Vector2Int>();

        //경로 역추적을 위한 딕셔너리, 각 노드의 gScore와 fScore를 저장하는 딕셔너리, 이미 평가된 노드를 저장하는 집합
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        //gScore는 시작점에서 해당 노드까지의 실제 비용, fScore는 gScore에 휴리스틱 비용을 더한 값
        var gScore = new Dictionary<Vector2Int, int>();

        //fScore는 gScore에 휴리스틱 비용을 더한 값
        var fScore = new Dictionary<Vector2Int, int>();

        //이미 평가된 노드들을 저장하는 집합
        var closedSet = new HashSet<Vector2Int>();


        //시작 노드를 openSet에 추가하고 gScore와 fScore 초기화
        openSet.Enqueue(start, 0);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);

        //8방향 이동을 위한 방향 벡터 배열
        Vector2Int[] dirs =
        {
            Vector2Int.down,
            Vector2Int.up,
            Vector2Int.left,
            Vector2Int.right,
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1)
        };

        //openSet에 후보 노드가 남아있는 동안 반복
        while (openSet.Count > 0)
        {
            //우선순위(fScore가 가장 낮은) 노드를 openSet에서 꺼내 현재 노드로 설정
            var current = openSet.Dequeue();

            //현재 노드가 목표 노드이면, cameFrom 딕셔너리를 사용하여 경로를 역추적하여 반환
            if (current == end)
            {
                return ReconstructPath(cameFrom, end);
            }

            //현재 노드를 closedSet에 추가하여 이미 평가된 노드로 표시(처리완료 표시)
            closedSet.Add(current);

            //이웃 노드 탐색
            foreach (var dir in dirs)
            {
                var neighbor = current + dir;

                //이미 평가된 노드인지 확인
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                //장애물이 있는지 확인
                if (neighbor != end && !IsValid(neighbor))
                {
                    continue;
                }

                //대각선 이동 시 코너컷팅 방지
                if (dir.x != 0 && dir.y != 0)
                {
                    if (!IsValid(new Vector2Int(current.x + dir.x, current.y)) ||
                        !IsValid(new Vector2Int(current.x, current.y + dir.y)))
                    {
                        continue;
                    }
                }

                int moveCost = (dir.x != 0 && dir.y != 0) ? 14 : 10; //대각선 이동 비용과 직선 이동 비용 구분
                int tentativeGScore = gScore[current] + moveCost;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + (Heuristic(neighbor, end) * 10);
                    openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }

        return null; //경로를 찾지 못함
    }

    /// <summary>
    /// 휴리스틱 함수는 현재 노드와 목표 노드 사이의 예상 비용을 계산하는 함수
    /// 휴리스틱(추정거리): 맨헤튼 거리(직선 이동이 아닌 격자 기반 이동에서 사용되는 거리 계산 방법)를 사용하여 계산
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    /// <summary>
    /// 해당 위치에 장애물이 있는지 확인하는 함수
    /// 해당 위치에서 CheckBox를 사용하여 장애물이 있는지 확인하는 IsValid 함수와 함께 사용하여 경로 탐색 시 장애물을 피할 수 있도록 함
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    //bool IsValid(Vector2Int pos)
    //{
    //    Vector3 worldPos = new Vector3(pos.x * cellSize, 1f, pos.y * cellSize);
    //
    //    bool isObstacle = Physics.CheckBox(worldPos, Vector3.one * (cellSize * checkSize), Quaternion.identity, obstacleLayer);
    //
    //    return !isObstacle;
    //}

    bool IsValid(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= gridManager.width || pos.y < 0 || pos.y >= gridManager.height)
            return false;

        return !gridManager.blocked[pos.y, pos.x];
    }

    /// <summary>
    /// cameFrom 딕셔너리를 사용하여 목표 노드(end)에서 시작 노드(start)까지의 경로를 역추적해서 리스트로 반환하는 함수
    /// </summary>
    /// <param name="cameFrom"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        var current = end;

        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(current);

        path.Reverse();

        return path;
    }
}
