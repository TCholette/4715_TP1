using System;
using UnityEngine;

public class PlayerEventManager : MonoBehaviour
{
    public static PlayerEventManager Instance;

    public static event Action OnPlayerDeath;
    public static event Action<int> OnPlayerCheckpoint;

    private void Awake()
    {
        OnPlayerDeath = null;
        OnPlayerCheckpoint = null;
        Instance = this;
        OnPlayerDeath += Die;

    }


    public void Die()
    {
        gameObject.transform.position = CheckPointManager.currentActiveCheckPoint.startZone.position;
    }

    public void RunDeath()
    {
        foreach (Action handler in OnPlayerDeath.GetInvocationList())
        {
            handler?.Invoke();
        }
    }

    public void RunCheckpoint(int index)
    {
        foreach (Action<int> handler in OnPlayerCheckpoint.GetInvocationList())
        {
            handler?.Invoke(index);
        }
    }
}
