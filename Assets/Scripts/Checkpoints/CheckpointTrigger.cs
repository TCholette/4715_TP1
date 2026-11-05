using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private bool _isStartOfLevel = false;
    [SerializeField] private bool _isEndOfLevel = false;
    private bool _canBeActivated = true;


    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite activated;
    [SerializeField] private Sprite deactivated;

    public void Deactivate()
    {
        sprite.sprite = deactivated;
    }
    private void Awake()
    {
        if (_isStartOfLevel)
        {
            CheckPointManager2.currentActiveCheckPoint = this;
            sprite.sprite = activated;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CheckPointManager2.currentActiveCheckPoint.Deactivate();
            sprite.sprite = activated;
            CheckPointManager2.currentActiveCheckPoint = this;
            if (_canBeActivated)
            {
                _canBeActivated = false;
                if (ScoreManager.Instance == null)
                {
                    return;
                }
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
}
