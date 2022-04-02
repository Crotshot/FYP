using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class RodgerController : PlayerController {

    protected bool overrideForward = false;

    protected override void FixedUpdate() {
        if (!ready || stunned)
            return;
        base.FixedUpdate();
        if (overrideForward) {
            Vector3 currentVelocity = transform.forward * Time.deltaTime * characterSpeed * force;
            currentVelocity = Helpers.Vector3Clamp(currentVelocity, -characterSpeed, characterSpeed);
            rb.velocity = currentVelocity;
        }
    }

    public override void EffectStart(string effect, float value) {
        if (effect.Equals("Rush")) {
            overrideForward = true;
            return;
        }
        base.EffectStart(effect, value);
    }

    public override void EffectEnd(string effect) {
        if (effect.Equals("Rush")) {
            overrideForward = false;
            return;
        }
        base.EffectEnd(effect);
    }
}
