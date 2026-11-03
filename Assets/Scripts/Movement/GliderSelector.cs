using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets._2D;

public class GliderSelector : MonoBehaviour
{
   [SerializeField] private PlatformerCharacter2D _character;

    [SerializeField] private List<Image> _images;
    [SerializeField] private List<GliderSelectorButton> _buttons;

    [SerializeField] private int _selectedIndex = 0;


    private void Awake() {
        foreach(GliderSelectorButton button in _buttons) {
            button.Init();
        }
        gameObject.SetActive(false);        
    }

    public void SetActiveGlider(GameObject glider, Sprite sprite ) {
        _character.SwitchGlider(glider);
        _images[0].sprite = _buttons[(_selectedIndex-1 + _buttons.Count) % _buttons.Count].sprite;
        _images[1].sprite = _buttons[_selectedIndex].sprite; ;
        _images[2].sprite = _buttons[(_selectedIndex+1)%_buttons.Count].sprite; ;
    }

    public void SetIndex(GliderSelectorButton button) {
        _selectedIndex = _buttons.IndexOf(button);
    }

    public void SwitchToNextGlider(int direction = 1) {
        _selectedIndex = (_selectedIndex + direction + _buttons.Count) % _buttons.Count;
        _buttons[_selectedIndex].SetGlider();
    }

}
