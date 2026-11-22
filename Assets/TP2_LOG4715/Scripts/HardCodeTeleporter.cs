using System.Collections.Generic;
using UnityEngine;

public class HardCodeTeleporter : MonoBehaviour
{
    [SerializeField] private TelevisionText televisionText;
    [SerializeField] private List<string> baseTexts;
    [SerializeField] private List<string> enterTexts;
    [SerializeField] private List<string> exitTexts;
    [SerializeField] private Transform tpPosition;
    [SerializeField] private bool canTp = false;

    private void Awake()
    {
        if (baseTexts.Count != 0)
        {
            televisionText.ReplaceTexts(baseTexts);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (enterTexts.Count != 0)
            {
                televisionText.ReplaceTexts(enterTexts);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (exitTexts.Count != 0)
            {
                televisionText.ReplaceTexts(exitTexts);
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (canTp)
        {
            if (collision.gameObject.CompareTag("Player") && Input.GetKey(KeyCode.W))
            {
                collision.gameObject.transform.position = tpPosition.position;
            }
        }
    }
}
