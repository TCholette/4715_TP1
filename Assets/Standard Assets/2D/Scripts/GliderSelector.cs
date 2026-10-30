using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets._2D;

public class GliderSelector : MonoBehaviour
{
   [SerializeField] private PlatformerCharacter2D _character;

    [SerializeField] private Image _image;

    public void SetActiveGlider(GameObject glider, Sprite sprite ) {
        _character.SwitchGlider(glider);
        _image.sprite = sprite;
    }
}
