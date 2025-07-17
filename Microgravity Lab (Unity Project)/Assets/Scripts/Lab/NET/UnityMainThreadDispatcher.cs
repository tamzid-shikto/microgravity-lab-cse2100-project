using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();
    private static UnityMainThreadDispatcher _instance = null;

    public static bool Exists()
    {
        return _instance != null;
    }

    public static void Initialize()
    {
        if (!Exists())
        {
            GameObject go = new GameObject("UnityMainThreadDispatcher");
            _instance = go.AddComponent<UnityMainThreadDispatcher>();
            DontDestroyOnLoad(go);
        }
    }

    public static void Enqueue(Action action)
    {
        if (!Exists())
        {
            Initialize();
        }

        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        Action action = null;

        lock (_executionQueue)
        {
            if (_executionQueue.Count > 0)
            {
                action = _executionQueue.Dequeue();
            }
        }

        action?.Invoke();
    }
}