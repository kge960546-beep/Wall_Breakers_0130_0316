using System.Collections.Generic;

/// <summary>
/// DAG 전체를 관리하는 클래스
/// - 노드 생성 및 고유성 보장
/// - 노드 조회
/// - 활성 노드 집계
/// </summary>
public class UpgradeDAGManager<T>
{
    // 노드 고유성을 보장하기 위한 맵
    // key = 노드 고유 ID
    private readonly Dictionary<string, UpgradeGraphNode<T>> nodeMap
        = new();

    // 외부에서는 읽기 전용으로 노출
    public IReadOnlyCollection<UpgradeGraphNode<T>> Nodes => nodeMap.Values;

    /// <summary>
    /// 노드 생성 (고유 ID 기반)
    /// 이미 존재하면 기존 노드 반환
    /// </summary>
    public UpgradeGraphNode<T> CreateNode(string id, T data)
    {
        // 이미 존재하면 기존 반환
        if (nodeMap.TryGetValue(id, out var existing))
            return existing;

        // 새 노드 생성
        var node = new UpgradeGraphNode<T>(data);

        // 등록
        nodeMap.Add(id, node);

        return node;
    }

    /// <summary>
    /// 특정 ID의 노드 조회
    /// </summary>
    public bool TryGetNode(string id, out UpgradeGraphNode<T> node)
    {
        return nodeMap.TryGetValue(id, out node);
    }

    /// <summary>
    /// 활성화된 노드들 반환 (중앙 집계용)
    /// </summary>
    public List<UpgradeGraphNode<T>> GetActivatedNodes()
    {
        var activated = new List<UpgradeGraphNode<T>>();

        foreach (var node in nodeMap.Values)
        {
            if (node.IsActivated)
                activated.Add(node);
        }

        return activated;
    }

    /// <summary>
    /// 전체 노드 초기화 (확장 대비용)
    /// </summary>
    public void ResetAll()
    {
        foreach (var node in nodeMap.Values)
        {
            // 필요 시 node.Reset() 추가 가능
        }
    }
}
