using UnityEngine;

public class Glider : AbsGlider
{
    public override void FixedUpdateStrategy() {
        TryGlide();
    }

    public override void JumpStrategy() {
        return;
    }

    public override void MoveStrategy(float direction) {
        return;
    }

    public override bool PerformAirDirectionCheckStrategy() {
        if (!isEnabled) return true;
        return !isGliding;
    }
}
