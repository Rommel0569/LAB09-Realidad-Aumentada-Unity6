using System.Collections;
using UnityEngine;

/// <summary>Plays the bundled alien's imported Legacy animation clips.</summary>
public sealed class SpaceBuddyAnimator : MonoBehaviour
{
    private Animation legacyAnimation;
    private AnimationClip[] clips;
    private AnimationClip idleClip;
    private AnimationClip playingClip;
    private Coroutine returnToIdle;

    private void Awake()
    {
        legacyAnimation = GetComponent<Animation>();
        if (legacyAnimation == null) legacyAnimation = GetComponentInChildren<Animation>();
        if (legacyAnimation == null) legacyAnimation = gameObject.AddComponent<Animation>();

        clips = Resources.LoadAll<AnimationClip>("AnimatedAlien");
        foreach (var clip in clips)
        {
            if (clip == null || clip.name.StartsWith("__", System.StringComparison.Ordinal)) continue;
            if (legacyAnimation.GetClip(clip.name) == null) legacyAnimation.AddClip(clip, clip.name);
        }

        idleClip = FindClip("Alien_IdleHold", "Alien_Idle", "Idle");
        if (idleClip == null)
        {
            Debug.LogError("AnimatedAlien.fbx no contiene clips de animación importados.");
            return;
        }

        PlayIdle();
    }

    public void PlayIdle() => PlayClip(idleClip, true);

    public void PlayWalk()
    {
        PlayClip(FindClip("Alien_Walk", "Walk", "Alien_Run", "Run"), true);
    }

    public void PlayJump()
    {
        PlayClip(FindClip("Alien_Jump", "RunningJump", "Jump"), false);
    }

    public void PlayClap()
    {
        PlayClip(FindClip("Alien_Clapping", "Clapping", "Alien_Punch"), false);
    }

    private AnimationClip FindClip(params string[] names)
    {
        if (clips == null) return null;
        foreach (var name in names)
        {
            foreach (var clip in clips)
            {
                if (clip != null && clip.name.IndexOf(name, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return clip;
            }
        }
        return null;
    }

    private void PlayClip(AnimationClip clip, bool loop)
    {
        if (clip == null || legacyAnimation == null)
        {
            if (clip == null) Debug.LogWarning("No se encontró el clip solicitado en AnimatedAlien.fbx.");
            return;
        }

        if (returnToIdle != null)
        {
            StopCoroutine(returnToIdle);
            returnToIdle = null;
        }

        playingClip = clip;
        legacyAnimation.Stop();
        var state = legacyAnimation[clip.name];
        state.wrapMode = loop ? WrapMode.Loop : WrapMode.Once;
        state.time = 0f;
        legacyAnimation.Play(clip.name);

        if (!loop)
            returnToIdle = StartCoroutine(ReturnToIdle(clip, clip.length));
    }

    private IEnumerator ReturnToIdle(AnimationClip action, float duration)
    {
        yield return new WaitForSeconds(Mathf.Max(duration, 0.1f));
        returnToIdle = null;
        if (playingClip == action && idleClip != null)
            PlayClip(idleClip, true);
    }
}
