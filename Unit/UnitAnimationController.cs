using UnityEngine;

public class UnitAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private UnitAnimationSet animationSet;

    public void Init(UnitAnimationSet set)
    {
        animationSet = set;
    }

    public void Play(AnimationState state, float fade = 0.1f)
    {
        string anim = animationSet.GetAnimationName(state);
        if (string.IsNullOrEmpty(anim)) return;

        animator.CrossFadeInFixedTime(anim, fade);
    }
}
