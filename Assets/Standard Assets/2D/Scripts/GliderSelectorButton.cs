using UnityEngine;
using UnityEngine.UI;

public class GliderSelectorButton : MonoBehaviour
{
    [SerializeField] private GliderSelector _selector;
    [SerializeField] private GameObject _glider;
    private Image _image;

    private void Awake() {
        _image = GetComponent<Image>();
    }

    public void SetGlider() {
        _selector.SetActiveGlider(_glider, _image.sprite);
    }

}
