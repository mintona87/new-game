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

        float animationTime = swordSlashClipLength + 1;/* PhotonNetwork.Time + 0.1; // Add a small buffer of 0.1 seconds*/

        player.pv.RPC("PlaySwordSlashEffectAttackerRPC", RpcTarget.All, targetScript.pv.ViewID, animationTime);

        yield return null;
    }

   
    [PunRPC]
    void PlaySwordSlashEffectAttackerRPC(int attackerViewID, float animationTime)
    {

        audioManager.Play("attackSound");

        // Find the attacker player using the PhotonView ID and get its PlayerManager script
        GameObject targetObj = PhotonView.Find(attackerViewID).gameObject;
        PlayerManager targetScript = targetObj.GetComponent<PlayerManager>();


        // Play animation
        targetScript.playerEffect.swordSlashEffect.enabled = true;
        targetScript.playerEffect.swordSlashAnimator.ResetTrigger("AnimationDone");
        targetScript.playerEffect.swordSlashAnimator.SetTrigger("PlaySwordSlash");

        StartCoroutine(DisableSlashEffect(animationTime, targetScript));

    }

    //IEnumerator PlaySwordSlashAnimationWithStartTime(PlayerManager targetScript, float startTime)
    //{
    //    //double currentTime = PhotonNetwork.Time;
    //    //float timeToWait = Mathf.Max(0f, (float)(startTime - currentTime));
    //}
    IEnumerator DisableSlashEffect(float animationTime, PlayerManager targetScript)
    {
        yield return new WaitForSeconds(animationTime);
        targetScript.playerEffect.swordSlashEffect.enabled = false;
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
    }

    // The rest of the code remains the same

    public IEnumerator PlayPlayer1ChargeEffect()
    {
        PlayerManager targetScript = player.playerCombat.targetScript;

        float chargeSlashClipLength = GetAnimationClipLength(targetScript.playerEffect.chargeAnimator, "ChargeAnimationClip");

        Debug.Log("clip length " + chargeSlashClipLength);

        float animationTime = chargeSlashClipLength +1;/* PhotonNetwork.Time + 0.1; // Add a small buffer of 0.1 seconds*/

        player.pv.RPC("PlayChargeRPC", RpcTarget.All, targetScript.pv.ViewID, animationTime);

        yield return null;
    }

    [PunRPC]
    void PlayChargeRPC(int attackerViewID, float animationTime)
    {
        audioManager.Play("chargeSound");

        // Find the attacker player using the PhotonView ID and get its PlayerManager script
        GameObject targetObj = PhotonView.Find(attackerViewID).gameObject;
        PlayerManager targetScript = targetObj.GetComponent<PlayerManager>();

        // Play animation
        targetScript.playerEffect.chargeEffect.enabled = true;
        targetScript.playerEffect.chargeAnimator.ResetTrigger("AnimationDone");
        targetScript.playerEffect.chargeAnimator.SetTrigger("PlayCharge");

        StartCoroutine(DisableChargeEffect(animationTime, targetScript));
    }

    IEnumerator DisableChargeEffect(float animationTime, PlayerManager targetScript)
    {
        yield return new WaitForSeconds(animationTime);
        targetScript.playerEffect.chargeEffect.enabled = false;
    }
}
