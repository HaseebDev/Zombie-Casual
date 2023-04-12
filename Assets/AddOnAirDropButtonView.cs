using System.Collections;
using System.Collections.Generic;
using QuickType.SkillDesign;
using UnityEngine;

public class AddOnAirDropButtonView : AddOnButtonView
{
    public Animator animator;

    private void UpdateAnimatorState()
    {
        SwitchAnimator(!SaveGameHelper.IsMaxDailyAddOnAirDrop());
    }

    public override void SetIsWaitingForUse(bool isWaiting)
    {
        base.SetIsWaitingForUse(isWaiting);
        if (!isWaiting)
        {
            UpdateAnimatorState();
        }
        else
        {
            SwitchAnimator(false);
        }
    }

    private void SwitchAnimator(bool isEnable)
    {
        if (isEnable)
        {
            bool isAvailable = !SaveGameHelper.IsMaxDailyAddOnAirDrop();
            animator.enabled = isAvailable;
            _iconShiny.Play();
        }
        else
        {
            animator.enabled = false;
            transform.localScale = Vector3.one;
            _iconShiny.Stop();
        }
    }
}
