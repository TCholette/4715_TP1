using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityStandardAssets._2D;

public abstract class Glider : MonoBehaviour
{
    private PlatformerCharacter2D _character;
    public void Init(PlatformerCharacter2D character) {
        _character = character;
    }

    abstract public void FixedUpdateStrategy();

    abstract public void MoveStrategy(float direction);

    abstract public void JumpStrategy();

    abstract public bool PerformAirDirectionCheckStrategy();
}
