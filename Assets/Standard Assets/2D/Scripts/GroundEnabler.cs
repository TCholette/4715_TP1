using System.Collections;
using UnityEngine;

public class GroundEnabler : MonoBehaviour
{
    [SerializeField] private GameObject _ground;
    private bool _isEnabled = true;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (_isEnabled) {
            _isEnabled = false;
            StartCoroutine(WaitForGround());
        }
    }

    private IEnumerator WaitForGround() {
        yield return new WaitForSeconds(2);
        _ground.SetActive(true);
    }
}
