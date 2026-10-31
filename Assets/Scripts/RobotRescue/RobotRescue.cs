using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class RobotRescue : MonoBehaviour
{
    private Vector3 _spawn;
    private bool _isFollowing = false;
    private GameObject _player;
    private int _index = 0;

    [SerializeField] private float speed;
    [SerializeField] private float baseDist;

    private void Awake()
    {
        _spawn = transform.position;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
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
        if (!_isFollowing)
        {
            _isFollowing = true;
            _index = player.GetComponent<RobotRescueHandler>().PrepareRobot(this);
            StartCoroutine(FollowTimer());
        }
    }

    private IEnumerator FollowTimer()
    {
        while (_isFollowing)
        {
            float dist = Vector3.Distance(_player.transform.position, transform.position);
            transform.position = Vector3.Lerp(transform.position, _player.transform.position, baseDist + Mathf.Pow(1.5f, dist) / Mathf.Pow(1.5f,speed * _index));
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void BringBack()
    {
        _isFollowing = false;
        _player = null;
        transform.position = _spawn;
    }
}
