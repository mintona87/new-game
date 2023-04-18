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

        // Get the length of the sword slash animation clip
        float swordSlashClipLength = GetAnimationClipLength(targetScript.playerEffect.swordSlashAnimator, "SwordSlashAnimationClip");

        Debug.Log("clip length " + swordSlashClipLength);

        // Call the RPC method to play the sword slash effect and animation on every client
        player.pv.RPC("PlaySwordSlashEffectRPC", RpcTarget.All, targetScript.pv.ViewID, swordSlashClipLength);

        yield return null;
    }

    [PunRPC]
    void PlaySwordSlashEffectRPC(int targetViewID, float delay)
    {
        // Play attack sound
        audioManager.Play("attackSound");

        // Find the target player using the PhotonView ID and get its PlayerManager script
        GameObject targetObj = PhotonView.Find(targetViewID).gameObject;
        PlayerManager targetScript = targetObj.GetComponent<PlayerManager>();

        // Enable sword slash effect and play animation
        targetScript.playerEffect.swordSlashEffect.enabled = true;
        targetScript.playerEffect.swordSlashAnimator.ResetTrigger("AnimationDone");
        targetScript.playerEffect.swordSlashAnimator.SetTrigger("PlaySwordSlash");

        // StartCoroutine to disable the effect after the duration of the animation
        StartCoroutine(DisableSwordSlashEffect(delay, targetScript));
    }


    IEnumerator DisableSwordSlashEffect(float delay, PlayerManager targetScript)
    {
        yield return new WaitForSeconds(delay);
        targetScript.playerEffect.swordSlashEffect.enabled = false;
    }

    public IEnumerator PlayHealEffect()
    {
        // play heal sound
        audioManager.Play("healSound");

        healEffect.enabled = true;
        healAnimator.ResetTrigger("AnimationDone");
        healAnimator.SetTrigger("PlayHeal");

        yield return new WaitForSeconds(2/*swordSlashAnimationClip.length*/);

        healEffect.enabled = false;
    }

    public IEnumerator PlayDefendEffect()
    {
        // play defend sound
        player.gameController.audioManager.Play("defendSound");

        defendEffect.enabled = true;
        defendAnimator.ResetTrigger("AnimationDone");
        defendAnimator.SetTrigger("PlayDefend");

        yield return new WaitForSeconds(2/*swordSlashAnimationClip.length*/);
        //yield return new WaitForSeconds(defendAnimationClip.length);

        defendEffect.enabled = false;
        player.isDefending = false;
    }


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
