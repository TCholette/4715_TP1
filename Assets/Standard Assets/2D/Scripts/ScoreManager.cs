using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int _maxScoreGain = 0;
    [SerializeField] private int _lossConstantDivisor = 1;
    [SerializeField] private int _timeTolerance = 10;
    [SerializeField] private bool _isTiming = true;
    [SerializeField] private TextMeshProUGUI _textTime;
    [SerializeField] private TextMeshProUGUI _textScore;
    private float _timeS = 0;
    public float TimeS { get { return _timeS; } }
    private int _currentScore = 0;
    private static ScoreManager _instance;
    public static ScoreManager Instance { get { return _instance; } }
    private void Awake()
    {
        // DontDestroyOnLoad(gameObject); 
        _instance = this;
    }

    public int SnapshotScoreGain()
    {
        float effectiveTimeS = Mathf.Max(0, _timeS - _timeTolerance);
        int score = (int)(_maxScoreGain * Mathf.Exp(-effectiveTimeS / _lossConstantDivisor));
        Debug.Log(score);
        return score;
    }

    private void StartTimer()
    {
        StartCoroutine(TimerCoroutine());
    }

    private void StopTimer()
    {
        _isTiming = false;
    }

    public void ToggleTimer()
    {
        if (_isTiming)
        {
            StopTimer();
        }
        else
        {
            StartTimer();
        }
    }

    private IEnumerator TimerCoroutine()
    {
        _timeS = 0;
        _isTiming = true;
        while (_isTiming)
        {
            yield return null;
            _timeS += Time.deltaTime;
            _textTime.text = _timeS.ToString("f3");
        }
        _currentScore += SnapshotScoreGain();
        _textScore.text = _currentScore.ToString();
    }


}
