using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private bool _isStartOfLevel = false;
    [SerializeField] private bool _isNoPoints= false;
    [SerializeField] private bool _isEndOfLevel = false;
    [SerializeField] private int _maxScoreGain;
    [SerializeField] private float _timeTolerance;
    private bool _canBeActivated = true;


    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite activated;
    [SerializeField] private Sprite deactivated;
    [SerializeField] public Transform startZone;
    [SerializeField] public int index;

    public void Deactivate()
    {
        if (sprite != null)
        {
            sprite.sprite = deactivated;
        }
    }
    private void Awake()
    {
        if (_isStartOfLevel)
        {
            CheckPointManager.currentActiveCheckPoint = this;
            if (sprite != null)
            {
                sprite.sprite = activated;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CheckPointManager.currentActiveCheckPoint.Deactivate();
            if (sprite != null)
            {
                sprite.sprite = activated;
            }
            CheckPointManager.currentActiveCheckPoint = this;
            PlayerEventManager.Instance.RunCheckpoint(index);
            if (_canBeActivated && !_isNoPoints)
            {
                _canBeActivated = false;
                if (ScoreManager.Instance == null)
                {
                    return;
                }
                if (!_isEndOfLevel)
                {
                    if (_maxScoreGain > 0)
                    {
                        ScoreManager.Instance.LapTimer(_maxScoreGain, _timeTolerance);
                    }
                    else
                    {
                        ScoreManager.Instance.StopTimer();
                    }
                }
                else
                {
                    ScoreManager.Instance.StopTimer();
                }
            }
        }
    }
}
