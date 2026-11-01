using System.Collections;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private bool _isEndOfLevel = false;
    private bool _isActive = true;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (_isActive) {
            if (!_isEndOfLevel) {
                ScoreManager.Instance.LapTimer();
                _isActive = false;
            } else {
                ScoreManager.Instance.StopTimer();
                _isActive = false;
            }
        }
    }
}
