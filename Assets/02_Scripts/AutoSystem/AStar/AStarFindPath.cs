using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarFindPath : MonoBehaviour
{
    [Header("세팅")]
    public LayerMask obstacleLayer; //장애물 레이어
    public GameObject floorSample; //바닥 오브젝트 샘플
    public float cellSize; //셀 크기
    public float checkSize = 0.35f; //충돌 체크 크기


    private void Awake()
    {
        if(floorSample != null)
        {
            cellSize = floorSample.GetComponent<Renderer>().bounds.size.x;
        }
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int y = Mathf.RoundToInt(worldPos.z / cellSize);
        return new Vector2Int(x, y);
    }
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        float x = gridPos.x * cellSize;
        float z = gridPos.y * cellSize;
        return new Vector3(x, 0f, z);
    }

    public List<Vector2Int> GetAstarPath(Vector2Int start, Vector2Int end)
    {
        var openSet = new PriorityQueue<Vector2Int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var gScore = new Dictionary<Vector2Int, int>();
        var fScore = new Dictionary<Vector2Int, int>();
        var closedSet = new HashSet<Vector2Int>();

        openSet.Enqueue(start, 0);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);        

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

        while(openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current == end)
            {
                return ReconstructPath(cameFrom, end);
            }
            
            closedSet.Add(current);

            foreach (var dir in dirs)
            {
                var neighbor = current + dir;

                if(!IsValid(neighbor) || closedSet.Contains(neighbor))
                {
                    continue;
                }
               
                int moveCost = (dir.x != 0 && dir.y != 0) ? 14 : 10; //대각선 이동 비용과 직선 이동 비용 구분
                int tentativeGScore = gScore[current] + moveCost;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + (Heuristic(neighbor, end)* 10);
                    openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }

        return null; //경로를 찾지 못함
    }

    int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    bool IsValid(Vector2Int pos)
    {
        Vector3 worldPos = new Vector3(pos.x * cellSize, 1f, pos.y * cellSize);

        bool isObstacle = Physics.CheckBox(worldPos, Vector3.one * (cellSize * checkSize), Quaternion.identity, obstacleLayer);

        return !isObstacle;
    }

    List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        var current = end;

        while(cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(current);

        path.Reverse();

        return path;
    }       
}
