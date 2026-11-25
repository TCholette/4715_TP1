using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private bool _isStartOfLevel = false;
    [SerializeField] private bool _isEndOfLevel = false;
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
