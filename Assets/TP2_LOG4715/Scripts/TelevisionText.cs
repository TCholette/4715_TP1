using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TelevisionText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float nextLetterTime;
    [SerializeField] private float nextWordTime;
    [SerializeField] private float nextMessageTime;
    [SerializeField] private List<Sprite> tvSprites;
    [SerializeField] private SpriteRenderer sprite;
    private List<string> _messages;

    private bool isSwitching = false;

    public void ReplaceTexts(List<string> text)
    {
        //this.text.color = color;
        _messages = text;
        this.text.text = "";
        stringIndex = 0;
        charIndex = 0;
        //sprite.sprite = tvSprite;
        isSwitching = true;
    }

    private int stringIndex = 0;
    private int charIndex = 0;

    private void Start()
    {
        if (_messages == null)
        {
            _messages = new();
        }
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        while (true)
        {
            isSwitching = false;
            if (_messages.Count <= stringIndex)
            {
                stringIndex = 0;
                yield return new WaitForSeconds(nextMessageTime);
                continue;
            }
            if (_messages[stringIndex].Length <= charIndex)
            {
                yield return new WaitForSeconds(nextMessageTime);
                if (isSwitching)
                {
                    continue;
                }
                sprite.sprite = tvSprites[0];
                text.color = Color.white;
                stringIndex++;
                stringIndex %= _messages.Count;
                charIndex = 0;
                text.text = "";
            }
            else
            {
                yield return new WaitForSeconds(nextLetterTime);
            }
            char character = _messages[stringIndex][charIndex];
            if (character == ' ')
            {
                yield return new WaitForSeconds(nextWordTime);
                if (isSwitching)
                {
                    continue;
                }
            }
            if (character == '$')
            {
                yield return new WaitForSeconds(nextWordTime);
                if (isSwitching)
                {
                    continue;
                }
                character = '\n';
            }
            if (character == '\\')
            {
                text.text += _messages[stringIndex][charIndex+1];
                charIndex += 2;
                continue;
            }
            if (character == '<')
            {
                switch (_messages[stringIndex][charIndex+1])
                {
                    case 'w':
                        text.color = Color.white;
                        break;
                    case 'r':
                        text.color = Color.red;
                        break;
                    case 'g':
                        text.color = Color.green;
                        break;
                    case 'y':
                        text.color = Color.yellow;
                        break;
                    case 'p':
                        text.color = Color.pink;
                        break;
                    case '.':
                        sprite.sprite = tvSprites[0];
                        break;
                    case '!':
                        sprite.sprite = tvSprites[1];
                        break;
                    default:
                        break;
                }
                charIndex += 3;
                continue;
            }

            text.text += character;
            charIndex++;
        }
    }
}
