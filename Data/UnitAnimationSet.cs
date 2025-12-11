using UnityEngine;

[CreateAssetMenu(fileName = "UnitAnimationSet", menuName = "Unit/Animation Set")]
public class UnitAnimationSet : ScriptableObject
{
    [System.Serializable]
    public struct AnimationEntry
    {
        public AnimationState state;
        public string animatorStateName;
    }

    public AnimationEntry[] animations;

    public string GetAnimationName(AnimationState state)
    {
        foreach (var a in animations)
            if (a.state == state)
                return a.animatorStateName;

        Debug.LogWarning($"Animation not found: {state}");
        return null;
    }
}
