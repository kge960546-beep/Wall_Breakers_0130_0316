using System.Collections.Generic;

/// <summary>
/// 범용 DAG 노드 클래스
/// - 부모 / 자식 관계 관리
/// - 활성 여부 관리
/// - 사이클 방지
/// - 데이터 타입은 제네릭으로 확장 가능
/// </summary>
public class UpgradeGraphNode<T>
{
    // 노드의 정체성 (생성 이후 변경 불가)
    public T Data { get; }

    // 부모 / 자식 리스트 참조 자체가 교체되지 않도록 보호
    private readonly List<UpgradeGraphNode<T>> parents = new();
    private readonly List<UpgradeGraphNode<T>> children = new();

    // 외부에서는 읽기만 가능하도록 노출 (구조 무결성 보호)
    public IReadOnlyList<UpgradeGraphNode<T>> Parents => parents;
    public IReadOnlyList<UpgradeGraphNode<T>> Children => children;

    // 노드 활성 상태 (외부에서 직접 수정 불가)
    public bool IsActivated { get; private set; }

    // 섹션 인덱스 추가
    public int SectionIndex { get; set; }

    /// <summary>
    /// 생성자
    /// 노드 생성 시 반드시 데이터가 정의되어야 함
    /// </summary>
    public UpgradeGraphNode(T data)
    {
        Data = data;
    }

    /// <summary>
    /// 부모-자식 연결
    /// this → child 방향 간선 생성
    /// - null 방지
    /// - 자기 자신 연결 방지
    /// - 사이클 생성 방지
    /// - 양방향 관계 동시 설정 (children / parents)
    /// </summary>
    public bool AddChild(UpgradeGraphNode<T> child)
    {
        if (child == null || child == this)
            return false;

        // 연결 시 순환 구조가 생기는지 사전 검사
        if (CreatesCycle(child))
            return false;

        // 중복 연결 방지
        if (!children.Contains(child))
        {
            children.Add(child);
            child.parents.Add(this); // 관계 무결성 유지
        }

        return true;
    }

    /// <summary>
    /// 부모 노드가 모두 활성화 되었는지 검사
    /// DAG 구조에서 활성 조건 판단용
    /// </summary>
    public bool CanActivate()
    {
        foreach (var parent in parents)
        {
            if (!parent.IsActivated)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 노드 활성화
    /// - 이미 활성화된 경우 중복 방지
    /// - 부모 조건을 만족해야 활성 가능
    /// </summary>
    public bool Activate()
    {
        // 이미 활성화된 노드는 재활성화 금지
        if (IsActivated)
            return false;

        // 부모 조건 미충족 시 활성 불가
        if (!CanActivate())
            return false;

        IsActivated = true;
        return true;
    }

    /// <summary>
    /// child를 연결할 경우 순환 구조가 발생하는지 검사
    /// child → ... → this 경로가 이미 존재하면 사이클 발생
    /// </summary>
    private bool CreatesCycle(UpgradeGraphNode<T> child)
    {
        return DFSContains(child, this, new HashSet<UpgradeGraphNode<T>>());
    }

    /// <summary>
    /// DFS 탐색
    /// start에서 target으로 도달 가능한지 확인
    /// - 방문 노드 기록하여 무한 루프 방지
    /// - DAG 보장을 위한 핵심 로직
    /// </summary>
    private bool DFSContains(
        UpgradeGraphNode<T> start,
        UpgradeGraphNode<T> target,
        HashSet<UpgradeGraphNode<T>> visited)
    {
        // 목표 노드에 도달하면 경로 존재
        if (start == target)
            return true;

        // 이미 방문한 노드는 재탐색하지 않음
        if (!visited.Add(start))
            return false;

        // 자식 방향으로 깊이 우선 탐색
        foreach (var next in start.children)
        {
            if (DFSContains(next, target, visited))
                return true;
        }

        return false;
    }
}
