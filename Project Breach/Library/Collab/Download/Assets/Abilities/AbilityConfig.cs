using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityConfig : ScriptableObject {

    [Header("Ability General")]
    [SerializeField] AnimationClip abilityAnimation;
    [SerializeField] GameObject overlay;
    [SerializeField] Color abilityColor = Color.white; // TODO change to image

    protected AbilityBehaviour behaviour;

    public abstract AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo);

    public void AttachAbilityTo(GameObject objectToAttachTo)
    {
        AbilityBehaviour behaviourComponent = GetBehaviourComponent(objectToAttachTo);
        behaviourComponent.SetConfig(this);
        behaviour = behaviourComponent;
    }

    public void Use()
    {
        behaviour.Use();
    }

    public void PlotAbility()
    {
        behaviour.PlotAbility();
    }

    public void DisablePlot()
    {
        behaviour.DisablePlot();
    }

    public AnimationClip GetAbilityAnimation()
    {
        return abilityAnimation;
    }

    public GameObject GetOverlay()
    {
        return overlay;
    }

    public Color GetAbilityColor()
    {
        return abilityColor;
    }
}
