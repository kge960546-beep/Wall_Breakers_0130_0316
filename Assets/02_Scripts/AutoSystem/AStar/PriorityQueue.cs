using System.Collections.Generic;

/// <summary>
/// 우선순위 큐 구현 클래스
/// Enqueue: 아이템과 우선순위를 함께 추가
/// Dequeue: 가장 높은 우선순위(가장 낮은 숫자)의 아이템을 탐색하고 제거(꺼냄)
/// </summary>
/// <typeparam name="T"></typeparam>
public class PriorityQueue<T>
{
    private List<(T item, int priority)> elements = new List<(T item, int priority)>();

    public int Count => elements.Count;

    public void Enqueue(T item, int priority)
    {
        elements.Add((item, priority));
    }
    public T Dequeue()
    {
        int bestIndex = 0;

        //가장 높은 우선순위(가장 낮은 숫자)의 아이템 탐색
        for (int i = 1; i < elements.Count; i++)
        {
            if (elements[i].priority < elements[bestIndex].priority)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].item;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}
