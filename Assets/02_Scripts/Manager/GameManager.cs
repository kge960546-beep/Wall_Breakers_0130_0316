using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

/// <summary>
/// GameManager
/// - 전역 게임 상태(Playing / Paused) 관리
/// - Time.timeScale 제어
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Dictionary<Type, object> services = new Dictionary<Type, object>();

    public enum GameState
    {
        Playing,
        Paused
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;

    // 게임 상태 변경 시 다른 시스템에 알리기 위한 전역 이벤트
    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        // 싱글톤 초기화 및 중복 방지
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 즉시 서비스 등록
        RegisterServices();

        Debug.Log("GameManager initailized and service registered");
    }

    private void Start()
    {
        // 시작 시 기본 상태 설정
        SetState(GameState.Playing);
    }

    public void PauseGame()
    {
        // UI, 광고 등으로 게임 일시 정지
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        // 일시 정지 해제 후 게임 재개
        SetState(GameState.Playing);
    }

    public bool IsPlaying()
    {
        // 현재 플레이 가능한 상태인지 여부
        return CurrentState == GameState.Playing;
    }

    private void SetState(GameState newState)
    {
        // 동일 상태 전환 방지
        if (CurrentState == newState)
            return;

        // 상태 변경 및 시간 흐름 제어
        CurrentState = newState;
        Time.timeScale = (newState == GameState.Paused) ? 0f : 1f;

        // 상태 변경 이벤트 전달
        OnGameStateChanged?.Invoke(CurrentState);
    }
    #region Service
    private void RegisterServices()
    {
        RegisterService(new CreditService());
        Debug.Log("[GameManager] CreditService registed successfully");
    }
    private void RegisterService<T>(T service)
    {
        var type = typeof(T);

        if(services.ContainsKey(type))
        {
            Debug.LogWarning($"[GameManager] Service already registed: {type}");
            return;
        }

        services.Add(type, service);
    }
    public T GetService<T>()
    {
        var type = typeof(T);

        if(services.TryGetValue(type, out var service))
            return (T)service;

        Debug.LogError($"[GameManger] Service not found : {type}");
        return default;
    }
    #endregion
}
