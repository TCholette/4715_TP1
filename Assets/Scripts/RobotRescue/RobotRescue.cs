using System;
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

    [SerializeField] private float speed;
    [SerializeField] private float baseDist;
    [SerializeField] private TextMeshPro text;

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

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        FollowPlayer(collision.gameObject);
    //    }
    //}

    public void FollowPlayer(GameObject player)
    {
        _player = player;
        _isFollowing = true;
        _index = player.GetComponent<RobotRescueHandler>().PrepareRobot(this);
        StartCoroutine(FollowTimer());
    }

    private IEnumerator FollowTimer()
    {
        if (_index == 0)
        {
            text.text = "Apportes-moi à un CheckPoint!";
        }
        else
        {
            text.text = "";
        }
        while (_isFollowing)
        {
            float dist = Vector3.Distance(_player.transform.position, transform.position);
            float test = Mathf.Exp(dist / (_index+1)) / Mathf.Exp(speed);
            transform.position = Vector3.Lerp(transform.position, _player.transform.position,test);
            yield return new WaitForSeconds(0.001f);
        }
    }

    public void BringBack()
    {
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
            yield return new WaitForSeconds(0.01f);
        }
        _isResetting = false;
        text.text = "À l'aide!";
    }
}
