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
            Vector3 currentVelocity = transform.forward * Time.deltaTime * characterSpeed * 50f;
            currentVelocity = Helpers.Vector3Clamp(currentVelocity, -characterSpeed, characterSpeed);
            rb.velocity = currentVelocity;
        }
    }

    public override void EffectStart(string effect) {
        if (effect.Equals("Rush")) {
            overrideForward = true;
        }
        else {
            base.EffectStart(effect);
        }
    }

    public override void EffectEnd(string effect) {
        if (effect.Equals("Rush")) {
            overrideForward = false;
        }
        else {
            base.EffectEnd(effect);
        }
    }
}
