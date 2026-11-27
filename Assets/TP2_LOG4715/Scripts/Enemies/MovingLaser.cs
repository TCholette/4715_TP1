using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingLaser : ToggleWithButton
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;
    [SerializeField] private float speed;
    [SerializeField] private float slowDownDistance = 1f;

    [SerializeField] private GameObject laser;

    private bool interruptTimer = false;

    private void Start()
    {
        PlayerEventManager.OnPlayerDeath += Respawn;
    }

    protected override void Open()
    {
        laser.SetActive(true);
        StartCoroutine(Advance());
    }

    protected override void Close()
    {
        laser.SetActive(false);
        interruptTimer = true;
    }
    private IEnumerator Advance()
    {
        interruptTimer = false;
        while (true)
        {
            yield return null;
            Vector2 target = end.position;
            Vector2 currentPos = transform.position;

            Vector2 dir = target - currentPos;
            float dirMagnitude = dir.magnitude;
            float lerpedSpeed = Mathf.Lerp(0, speed, dirMagnitude / slowDownDistance);
            transform.position = currentPos + dir.normalized * lerpedSpeed * Time.deltaTime;

            if (interruptTimer)
            {
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerEventManager>().RunDeath();
        }
    }

    public void SetZone(Transform zoneStart)
    {
        start = zoneStart;
    }
    public void Stop()
    {
        interruptTimer = true;
        start = transform;
    }

    private void Respawn()
    {
        transform.position = start.position;
    }
}