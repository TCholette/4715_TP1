using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int _lossConstantDivisor = 1;
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
        _instance = this;
    }

    public void AddScore(int score) {
        _currentScore += score;
        _textScore.text = "Score: " + _currentScore.ToString();

    }

    public int SnapshotScoreGain(int maxScoreGain, float timeTolerance)
    {
        float effectiveTimeS = Mathf.Max(0, _timeS - timeTolerance);
        int score = (int)(maxScoreGain * Mathf.Exp(-effectiveTimeS / _lossConstantDivisor));
        //Debug.Log(score);
        return score;
    }

    public void LapTimer(int maxScoreGain, float timeTolerance)
    {
        StartCoroutine(LapTimerCoroutine(maxScoreGain, timeTolerance));
    }

    public void StopTimer() {
        _isTiming = false;
        _showTimer = false;
        _textTime.text = "";
    }

    private IEnumerator LapTimerCoroutine(int maxScoreGain, float timeTolerance)
    {
        _showTimer = true;
        _isTiming = false;
        yield return new WaitForEndOfFrame();
        if (_timer != null) {
            StopCoroutine(_timer);
        }
        _timer = StartCoroutine(TimerCoroutine(maxScoreGain, timeTolerance));
    }

    private IEnumerator TimerCoroutine(int maxScoreGain, float timeTolerance)
    {
        _isTiming = true;
        _timeS = 0;
        while (_isTiming)
        {
            yield return null;
            _timeS += Time.deltaTime;
            if (_showTimer) _textTime.text = "Temps: " + _timeS.ToString("f3") + " - " + SnapshotScoreGain(maxScoreGain, timeTolerance);
        }
        _currentScore += SnapshotScoreGain(maxScoreGain, timeTolerance);
        _textScore.text = "Score: " + _currentScore.ToString();
    }
}
