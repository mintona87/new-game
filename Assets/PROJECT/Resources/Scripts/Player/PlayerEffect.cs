using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerEffect : MonoBehaviour
{
    PlayerManager player;
    public Image swordSlashEffect;
    public Image healEffect;
    public Image defendEffect;
    public Image chargeEffect;

    public Animator swordSlashAnimator;
    public Animator healAnimator;
    public Animator defendAnimator;
    public Animator chargeAnimator;

    AudioManager audioManager;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        audioManager = player.gameController.audioManager;
    }


    float GetAnimationClipLength(Animator animator, string clipName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        return 0;
    }

    public IEnumerator PlaySwordSlashEffect()
    {
        PlayerManager targetScript = player.playerCombat.targetScript;

        float swordSlashClipLength = GetAnimationClipLength(targetScript.playerEffect.swordSlashAnimator, "SwordSlashAnimationClip");

        Debug.Log("clip length " + swordSlashClipLength);

        double startTime = PhotonNetwork.Time + 0.1; // Add a small buffer of 0.1 seconds

        // Play animation on the local player (attacker) as well
        player.pv.RPC("PlaySwordSlashEffectAttackerRPC", RpcTarget.All, player.pv.ViewID, startTime);

        player.pv.RPC("PlaySwordSlashEffectRPC", RpcTarget.All, targetScript.pv.ViewID, swordSlashClipLength, startTime);

        yield return null;
    }

    [PunRPC]
    void PlaySwordSlashEffectRPC(int targetViewID, float delay, double startTime)
    {
        // Play attack sound
        audioManager.Play("attackSound");

        // Find the target player using the PhotonView ID and get its PlayerManager script
        GameObject targetObj = PhotonView.Find(targetViewID).gameObject;
        PlayerManager targetScript = targetObj.GetComponent<PlayerManager>();

        // Enable sword slash effect
        targetScript.playerEffect.swordSlashEffect.enabled = true;

        // Wait for the synchronized start time before playing the animation
        StartCoroutine(PlaySwordSlashAnimationWithStartTime(targetScript, startTime));
    }
    [PunRPC]
    void PlaySwordSlashEffectAttackerRPC(int attackerViewID, double startTime)
    {
        // Find the attacker player using the PhotonView ID and get its PlayerManager script
        GameObject attackerObj = PhotonView.Find(attackerViewID).gameObject;
        PlayerManager attackerScript = attackerObj.GetComponent<PlayerManager>();

        // Wait for the synchronized start time before playing the animation on the attacker
        StartCoroutine(PlaySwordSlashAnimationWithStartTime(attackerScript, startTime));
    }

    IEnumerator PlaySwordSlashAnimationWithStartTime(PlayerManager targetScript, double startTime)
    {
        double currentTime = PhotonNetwork.Time;
        float timeToWait = Mathf.Max(0f, (float)(startTime - currentTime));

        yield return new WaitForSeconds(timeToWait);

        // Play animation
        targetScript.playerEffect.swordSlashAnimator.ResetTrigger("AnimationDone");
        targetScript.playerEffect.swordSlashAnimator.SetTrigger("PlaySwordSlash");
    }
    public IEnumerator PlayHealEffect()
    {
        float healClipLength = GetAnimationClipLength(healAnimator, "HealEffectAnimationClip");

        player.pv.RPC("PlayHealEffectRPC", RpcTarget.All, player.pv.ViewID, healClipLength);

        yield return null;
    }

    [PunRPC]
    void PlayHealEffectRPC(int targetViewID, float delay)
    {
        audioManager.Play("healSound");

        GameObject targetObj = PhotonView.Find(targetViewID).gameObject;
        PlayerManager targetScript = targetObj.GetComponent<PlayerManager>();

        targetScript.playerEffect.healEffect.enabled = true;
        targetScript.playerEffect.healAnimator.ResetTrigger("AnimationDone");
        targetScript.playerEffect.healAnimator.SetTrigger("PlayHeal");

        StartCoroutine(DisableHealEffect(delay, targetScript));
    }

    IEnumerator DisableHealEffect(float delay, PlayerManager targetScript)
    {
        yield return new WaitForSeconds(delay);
        targetScript.playerEffect.healEffect.enabled = false;
    }

    public IEnumerator PlayDefendEffect()
    {
        // Get the length of the defend animation clip
        float defendClipLength = GetAnimationClipLength(defendAnimator, "DefendAnimationClip");

        // Call the RPC method to play the defend effect and animation on every client
        player.pv.RPC("PlayDefendEffectRPC", RpcTarget.All, player.pv.ViewID, defendClipLength);

        yield return null;
    }

    [PunRPC]
    void PlayDefendEffectRPC(int targetViewID, float delay)
    {
        // Play defend sound
        audioManager.Play("defendSound");

        // Find the target player using the PhotonView ID and get its PlayerManager script
        GameObject targetObj = PhotonView.Find(targetViewID).gameObject;
        PlayerManager targetScript = targetObj.GetComponent<PlayerManager>();

        // Enable defend effect and play animation
        targetScript.playerEffect.defendEffect.enabled = true;
        targetScript.playerEffect.defendAnimator.ResetTrigger("AnimationDone");
        targetScript.playerEffect.defendAnimator.SetTrigger("PlayDefend");

        // StartCoroutine to disable the effect after the duration of the animation
        StartCoroutine(DisableDefendEffect(delay, targetScript));
    }
    IEnumerator DisableDefendEffect(float delay, PlayerManager targetScript)
    {
        yield return new WaitForSeconds(delay);
        targetScript.playerEffect.defendEffect.enabled = false;
        targetScript.SetIsDefending(false);
    }

    // The rest of the code remains the same

    public IEnumerator PlayPlayer1ChargeEffect()
    {
        // play charge sound
        //audioChargeSource.PlayOneShot(chargeSound);
        audioManager.Play("chargeSound");

        chargeEffect.enabled = true;
        chargeAnimator.ResetTrigger("AnimationDone");
        chargeAnimator.SetTrigger("PlayCharge");

        //yield return new WaitForSeconds(chargeAnimationClip.length);
        yield return new WaitForSeconds(2/*swordSlashAnimationClip.length*/);

        chargeEffect.enabled = false;
    }
}
