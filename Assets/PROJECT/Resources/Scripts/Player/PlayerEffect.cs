using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    public IEnumerator PlaySwordSlashEffect()
    {
        // play attack sound
        //audioAttackSource.PlayOneShot(attackSound);
        PlayerManager targetScript = player.playerCombat.targetScript;


        audioManager.Play("attackSound");
        targetScript.playerEffect.swordSlashEffect.enabled = true;
        targetScript.playerEffect.swordSlashAnimator.ResetTrigger("AnimationDone");
        targetScript.playerEffect.swordSlashAnimator.SetTrigger("PlaySwordSlash");

        yield return new WaitForSeconds(2/*swordSlashAnimationClip.length*/);

        targetScript.playerEffect.enabled = false;
    }

    public IEnumerator PlayHealEffect()
    {
        // play heal sound
        //audioHealSource.PlayOneShot(healSound);
        audioManager.Play("healSound");

        healEffect.enabled = true;
        healAnimator.ResetTrigger("AnimationDone");
        healAnimator.SetTrigger("PlayHeal");

        //yield return new WaitForSeconds(healAnimationClip.length);
        yield return new WaitForSeconds(2/*swordSlashAnimationClip.length*/);

        healEffect.enabled = false;
    }

    public IEnumerator PlayDefendEffect()
    {
        // play defend sound
        player.gameController.audioManager.Play("defendSound");
        //audioDefendSource.PlayOneShot(defendSound);

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
