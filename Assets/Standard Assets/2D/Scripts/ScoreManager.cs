using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int _maxScoreGain = 0;
    [SerializeField] private int _lossConstantDivisor = 1;
    [SerializeField] private int _timeTolerance = 10;
    [SerializeField] private TextMeshProUGUI _textTime;
    [SerializeField] private TextMeshProUGUI _textScore;
    [SerializeField] private bool _showTimer = false;
    private Coroutine _timer;
    private bool _isTiming = false;
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

    public void LapTimer()
    {
        StartCoroutine(LapTimerCoroutine());
    }

    public void StopTimer() {
        _isTiming = false;
        _showTimer = false;
        _textTime.text = "";
    }

    private IEnumerator LapTimerCoroutine() {
        _isTiming = false;
        yield return new WaitForEndOfFrame();
        if (_timer != null) {
            StopCoroutine(_timer);
        }
        _timer = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        _isTiming = true;
        _timeS = 0;
        while (_isTiming)
        {
            yield return null;
            _timeS += Time.deltaTime;
            if (_showTimer) _textTime.text = "Temps: " + _timeS.ToString("f3") + " - " + SnapshotScoreGain();
        }
        _currentScore += SnapshotScoreGain();
        _textScore.text = "Score: " + _currentScore.ToString();
    }
}
