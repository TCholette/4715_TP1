using System.Collections;
using TMPro;
using UnityEngine;

public class RobotRescue : MonoBehaviour
{
    private Vector3 _spawn;
    private bool _isFollowing = false;
    private bool _isResetting = false;
    private GameObject _player;
    private int _index = 0;

    [SerializeField] private float robotSpeed;
    [SerializeField] private TextMeshPro text;
    [SerializeField] private Animator anim;


    private void Awake()
    {
        _spawn = transform.position;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_isResetting && !_isFollowing)
        {
            FollowPlayer(collision.gameObject);
        }
    }
    public void FollowPlayer(GameObject player)
    {
        _player = player;
        _isFollowing = true;
        _index = player.GetComponent<RobotRescueHandler>().PrepareRobot(this);
        StartCoroutine(FollowTimer());
    }

    private IEnumerator FollowTimer()
    {
        anim.SetBool("isFollowing", true);
        if (_index == 0)
        {
            text.text = "Apporte-moi à un CheckPoint!";
        }
        else
        {
            text.text = "";
        }
        while (_isFollowing)
        {
            float dist = Vector3.Distance(_player.transform.position, transform.position);
            float test = Mathf.Exp(dist / (_index+1)) / Mathf.Exp(robotSpeed);
            transform.position = Vector3.Lerp(transform.position, _player.transform.position,test);
            yield return null;
        }
    }

    public void BringBack()
    {
        anim.SetBool("isFollowing", false);
        text.text = "Nooon!";
        if (!_isResetting)
        {
            _isResetting = true;
            _isFollowing = false;
            _player = null;
            StartCoroutine(ResetTimer());
        }
    }

    private IEnumerator ResetTimer()
    {
        int i = 0;
        Vector3 basePos = transform.position;
        while (Vector3.Distance(transform.position, _spawn) > 0.1f)
        {
            transform.position = Vector3.Lerp(basePos, _spawn, i/100f);
            i++;
            yield return null;
        }
        _isResetting = false;
        text.text = "!";
    }
}
