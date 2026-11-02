using System.Collections;
using UnityEngine;

public class CheckpointTrigger : CheckPointTriggerBase
{
    private void Awake()
    {
        if (_isStartOfLevel)
        {
            //CheckPointManager2.currentActiveCheckPoint = this;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (_isActive)
        {
            Debug.Log($"Collision with: " + collision.gameObject.name + "; " + collision.gameObject.layer);
            _isActive = false;
            //CheckPointManager2.currentActiveCheckPoint = this;
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
