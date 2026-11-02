using System.Collections;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private bool _isStartOfLevel = false;
    [SerializeField] private bool _isEndOfLevel = false;
    private bool _isActive = true;

    private void Awake()
    {
        if (_isStartOfLevel)
        {
            CheckPointManager2.currentActiveCheckPoint = this;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (_isActive)
        {
            _isActive = false;
            CheckPointManager2.currentActiveCheckPoint = this;
            if (!_isEndOfLevel) 
            {
                ScoreManager.Instance.LapTimer();
            } 
            else 
            {
                ScoreManager.Instance.StopTimer();
            }
        }
    }
}
