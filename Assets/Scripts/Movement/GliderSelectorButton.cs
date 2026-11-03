using UnityEngine;
using UnityEngine.UI;

public class GliderSelectorButton : MonoBehaviour
{
    [SerializeField] private GliderSelector _selector;
    [SerializeField] private GameObject _glider;
    private Image _image;

    public Sprite sprite { get { return _image.sprite; } }
    public void Init() {
        _image = GetComponent<Image>();
    }

    public void SetGlider()
    {
        _selector.SetIndex(this);
        _selector.SetActiveGlider(_glider, _image.sprite);
    }

}
