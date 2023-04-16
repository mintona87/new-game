using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI player1ActionText;
    [SerializeField] private TextMeshProUGUI player2ActionText;

    
    [SerializeField] private TextMeshProUGUI HealthText;
    [SerializeField] private AudioSource audioAttackSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioSource audioHealSource;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioSource audioChargeSource;
    [SerializeField] private AudioClip chargeSound;
    [SerializeField] private AudioSource audioDefendSource;
    [SerializeField] private AudioClip defendSound;
    public AudioSource backgroundMusic;

    [SerializeField] private PlayerManager player1Script;
    [SerializeField] private PlayerManager player2Script;

    public int player1Honor = 0;
    public int player2Honor = 0;
    [SerializeField] private TextMeshProUGUI honorText;
    private float[] playerHonor = new float[2];



    public int Turn = 0;

    public Image Player1SwordSlashEffect;
    public Image Player2SwordSlashEffect;
    public Animator swordSlashAnimatorP1;
    public Animator swordSlashAnimatorP2;
    public AnimationClip swordSlashAnimationClip;
    public Image Player1HealEffect;
    public Image Player2HealEffect;
    public Animator healAnimatorP1;
    public Animator healAnimatorP2;
    public AnimationClip healAnimationClip;
    public Image Player1DefendEffect;
    public Image Player2DefendEffect;
    public Animator defendAnimatorP1;
    public Animator defendAnimatorP2;
    public AnimationClip defendAnimationClip;
    public Image Player1ChargeEffect;
    public Image Player2ChargeEffect;
    public Animator chargeAnimatorP1;
    public Animator chargeAnimatorP2;
    public AnimationClip chargeAnimationClip;

    public Button player1AttackButton;
    public Button player1HealButton;
    public Button player1DefendButton;
    public Button player1ChargeButton;

    public Button player2AttackButton;
    public Button player2HealButton;
    public Button player2DefendButton;
    public Button player2ChargeButton;

    private int player1TurnsSinceCharge = 6;
    private int player2TurnsSinceCharge = 6;

    private bool isGameOver = false;

    public AudioManager audioManager;
    public List<PlayerManager> playerList = new List<PlayerManager>();


    private void Awake()
    {
        audioManager = GetComponent<AudioManager>();
    }


    void Start()
    {
        //UpdateHealthUI();
        //UpdateHonorUI();
        //backgroundMusic.Play();
        //UpdateChargeButtons();
        //SwitchPlayerTurn();
        playerList.Add(GameObject.Find("Player1").GetComponent<PlayerManager>());
        playerList.Add(GameObject.Find("Player2").GetComponent<PlayerManager>());
        SwitchPlayerTurn();
    }

    public void UpdateHealthUI()
    {
        HealthText.text = $"P1 HP: {player1Script.HP}     |     P2 HP: {player2Script.HP}";
    }

   public void UpdateHonorUI()
   {
        honorText.text = $"P1 Honor: {playerHonor[0]} | P2 Honor: {playerHonor[1]}";
   }



   public void OnPlayer1AttackButtonClicked()
    {
        if (player1Script.HasLost()) return;

        int damage = player1Script.Attack();

        if (!player2Script.Dodge()) // Check if Player 2 dodged the attack
        {
            player2Script.ChangeHP(-damage);
            StartCoroutine(ShowActionText($"Player 1 dealt {damage} damage!", player1ActionText));
        }
        else
        {
            StartCoroutine(ShowActionText("Player 1 missed the attack!", player1ActionText));
        }

        Turn++;
        player2TurnsSinceCharge++;
        StartCoroutine(PlayPlayer1SwordSlashEffect());

        UpdateHealthUI();
        UpdateChargeButtons();
        SwitchPlayerTurn();
    }


public void OnPlayer2AttackButtonClicked()
{
    if (player2Script.HasLost()) return;

    int damage = player2Script.Attack();

    if (!player1Script.Dodge()) // Check if Player 2 dodged the attack
    {
        player1Script.ChangeHP(-damage);
        StartCoroutine(ShowActionText($"Player 2 dealt {damage} damage!", player2ActionText));
    }
    else
    {
        StartCoroutine(ShowActionText("Player 2 missed the attack!", player2ActionText));
    }

    Turn++;
    player1TurnsSinceCharge++;
    StartCoroutine(PlayPlayer2SwordSlashEffect());

    UpdateHealthUI();
    UpdateChargeButtons();
    SwitchPlayerTurn();
}

    private IEnumerator PlayPlayer1SwordSlashEffect()
    {
        // play attack sound
        audioAttackSource.PlayOneShot(attackSound);

        Player1SwordSlashEffect.enabled = true;
        swordSlashAnimatorP1.ResetTrigger("AnimationDone");
        swordSlashAnimatorP1.SetTrigger("PlaySwordSlash");

        yield return new WaitForSeconds(swordSlashAnimationClip.length);

        Player1SwordSlashEffect.enabled = false;
    }

    private IEnumerator PlayPlayer2SwordSlashEffect()
    {
        // play attack sound
        audioAttackSource.PlayOneShot(attackSound);

        Player2SwordSlashEffect.enabled = true;
        swordSlashAnimatorP2.ResetTrigger("AnimationDone");
        swordSlashAnimatorP2.SetTrigger("PlaySwordSlash");

        yield return new WaitForSeconds(swordSlashAnimationClip.length);

        Player2SwordSlashEffect.enabled = false;
    }

    public void OnPlayer1HealButtonClicked()
    {
        if (player1Script.HasLost()) return;

        int healAmount = player1Script.Heal();
        player1Script.ChangeHP(healAmount);
        if (player1Script.HP > player1Script.MaxHP)
        {
            player1Script.HP = player1Script.MaxHP;
        }
        Turn++;
        player2TurnsSinceCharge++;
        StartCoroutine(PlayPlayer1HealEffect());

        UpdateHealthUI();
        UpdateChargeButtons();
        SwitchPlayerTurn();

        StartCoroutine(ShowActionText($"Player 1 healed for {healAmount}!", player1ActionText));


    }

    public void OnPlayer2HealButtonClicked()
    {
        if (player2Script.HasLost()) return;

        int healAmount = player1Script.Heal();
        player2Script.ChangeHP(healAmount);
        if (player2Script.HP > player2Script.MaxHP)
        {
            player2Script.HP = player2Script.MaxHP;
        }
        Turn++;
        player1TurnsSinceCharge++;
        StartCoroutine(PlayPlayer2HealEffect());

        UpdateHealthUI();
        UpdateChargeButtons();
        SwitchPlayerTurn();

        StartCoroutine(ShowActionText($"Player 2 healed for {healAmount}!", player2ActionText));


    }

    private IEnumerator PlayPlayer1HealEffect()
    {
        // play heal sound
        audioHealSource.PlayOneShot(healSound);

        Player1HealEffect.enabled = true;
        healAnimatorP1.ResetTrigger("AnimationDone");
        healAnimatorP1.SetTrigger("PlayHeal");

        yield return new WaitForSeconds(healAnimationClip.length);

        Player1HealEffect.enabled = false;
    }

    private IEnumerator PlayPlayer2HealEffect()
    {
        // play heal sound
        audioHealSource.PlayOneShot(healSound);

        Player2HealEffect.enabled = true;
        healAnimatorP2.ResetTrigger("AnimationDone");
        healAnimatorP2.SetTrigger("PlayHeal");

        yield return new WaitForSeconds(healAnimationClip.length);

        Player2HealEffect.enabled = false;
    }

    public void OnPlayer1DefendButtonClicked()
    {
        if (player1Script.HasLost()) return;
        player1Script.isDefending = true;
        Turn++;
        player2TurnsSinceCharge++;
        StartCoroutine(PlayPlayer1DefendEffect());

        UpdateHealthUI();
        UpdateChargeButtons();
        SwitchPlayerTurn();

    }

    public void OnPlayer2DefendButtonClicked()
    {
        if (player2Script.HasLost()) return;
        player2Script.isDefending = true;
        Turn++;
        player1TurnsSinceCharge++;
        StartCoroutine(PlayPlayer2DefendEffect());

        UpdateHealthUI();
        UpdateChargeButtons();
        SwitchPlayerTurn();

        
    }

    private IEnumerator PlayPlayer1DefendEffect()
    {
        // play defend sound
        audioDefendSource.PlayOneShot(defendSound);

        Player1DefendEffect.enabled = true;
        defendAnimatorP1.ResetTrigger("AnimationDone");
        defendAnimatorP1.SetTrigger("PlayDefend");

        yield return new WaitForSeconds(defendAnimationClip.length);
        
        Player1DefendEffect.enabled = false;
        player1Script.isDefending = false;
    }

    private IEnumerator PlayPlayer2DefendEffect()
    {
        // play defend sound
        audioDefendSource.PlayOneShot(defendSound);

        Player2DefendEffect.enabled = true;
        defendAnimatorP2.ResetTrigger("AnimationDone");
        defendAnimatorP2.SetTrigger("PlayDefend");

        yield return new WaitForSeconds(defendAnimationClip.length);
        
        Player2DefendEffect.enabled = false;
        player2Script.isDefending = false;
    }

public void OnPlayer1ChargeButtonClicked()
{
    if (player1Script.HasLost()) return;
    if (player1Script.HasCharged)
    {
        // Show error message that Charge is on cooldown
        return;
    }
    
    int damage = player1Script.Charge(player2Script);
    player2Script.ChangeHP(-damage);

    Turn++;
    player1TurnsSinceCharge = 0; // Reset the counter
    
    StartCoroutine(PlayPlayer1ChargeEffect());

    UpdateHealthUI();
    UpdateChargeButtons();
    SwitchPlayerTurn();

    StartCoroutine(ShowActionText($"Player 1 dealt {damage} damage!", player1ActionText));
}


public void OnPlayer2ChargeButtonClicked()
{
    if (player2Script.HasLost()) return;
    if (player2Script.HasCharged)
    {
        // Show error message that Charge is on cooldown
        return;
    }
    int damage = player2Script.Charge(player1Script);
    player1Script.ChangeHP(-damage);

    Turn++;
    player2TurnsSinceCharge = 0; // Reset the counter

    StartCoroutine(PlayPlayer2ChargeEffect());

    UpdateHealthUI();
    UpdateChargeButtons();
    SwitchPlayerTurn();

    StartCoroutine(ShowActionText($"Player 2 dealt {damage} damage!", player2ActionText));


}


// Coroutine to show action text and fade it away
public IEnumerator ShowActionText(string text, TextMeshProUGUI textComponent)
{
    float duration = 1.5f; // How long the text should stay visible
    float fadeDuration = 0.5f; // How long the fade in/out should take

    // Set the text and alpha value to 0
    textComponent.text = text;
    textComponent.canvasRenderer.SetAlpha(0f);

    // Fade in
    textComponent.CrossFadeAlpha(1f, fadeDuration, false);

    // Wait for the duration
    yield return new WaitForSeconds(duration);

    // Fade out
    textComponent.CrossFadeAlpha(0f, fadeDuration, false);
}




private void UpdateChargeButtons()
{
    if (Turn % 2 == 1)
    {
        player1ChargeButton.interactable = player1TurnsSinceCharge >= 6;
        player2ChargeButton.interactable = false;
        
        if (player1TurnsSinceCharge >= 6)
        {
            player1Script.ResetCharge();
        }
    }
    else
    {
        player1ChargeButton.interactable = false;
        player2ChargeButton.interactable = player2TurnsSinceCharge >= 6;

        if (player2TurnsSinceCharge >= 6)
        {
            player2Script.ResetCharge();
        }
    }
}



    private IEnumerator PlayPlayer1ChargeEffect()
    {
        // play charge sound
        audioChargeSource.PlayOneShot(chargeSound);

        Player1ChargeEffect.enabled = true;
        chargeAnimatorP1.ResetTrigger("AnimationDone");
        chargeAnimatorP1.SetTrigger("PlayCharge");

        yield return new WaitForSeconds(chargeAnimationClip.length);

        Player1ChargeEffect.enabled = false;
    }

    private IEnumerator PlayPlayer2ChargeEffect()
    {
        // play charge sound
        audioChargeSource.PlayOneShot(chargeSound);

        Player2ChargeEffect.enabled = true;
        chargeAnimatorP2.ResetTrigger("AnimationDone");
        chargeAnimatorP2.SetTrigger("PlayCharge");

        yield return new WaitForSeconds(chargeAnimationClip.length);

        Player2ChargeEffect.enabled = false;
    }

    // Use Update to enable/disable buttons based on whose turn it is.


    public void SwitchPlayerTurn()
    {
        //if (Turn % 2 == 0)
        //{
        //    player1AttackButton.interactable = false;
        //    player1HealButton.interactable = false;
        //    player1DefendButton.interactable = false;
        //    player1ChargeButton.interactable = false;

        //    player2AttackButton.interactable = true;
        //    player2HealButton.interactable = true;
        //    player2DefendButton.interactable = true;
        //    player2ChargeButton.interactable = player2TurnsSinceCharge >= 6;

        //     if (player2Script.isStunned)
        //      {
        //        Turn++;
        //        // Skip opponent's turn and allow player 1 to take another action
        //        player2Script.isStunned = false;
        //        player1AttackButton.interactable = true;
        //        player1HealButton.interactable = true;
        //        player1DefendButton.interactable = true;
        //        player1ChargeButton.interactable = player1TurnsSinceCharge >= 6;

        //        player2AttackButton.interactable = false;
        //        player2HealButton.interactable = false;
        //        player2DefendButton.interactable = false;
        //        player2ChargeButton.interactable = false;

        //        StartCoroutine(ShowActionText("Player 2 is stunned!", player2ActionText));
        //        StartCoroutine(ShowActionText(" ", player1ActionText));
        //    }
        //}
        //else
        //{
        //    player1AttackButton.interactable = true;
        //    player1HealButton.interactable = true;
        //    player1DefendButton.interactable = true;
        //    player1ChargeButton.interactable = player1TurnsSinceCharge >= 6;

        //    player2AttackButton.interactable = false;
        //    player2HealButton.interactable = false;
        //    player2DefendButton.interactable = false;
        //    player2ChargeButton.interactable = false;

        //    if (player1Script.isStunned)
        //    {
        //        Turn++;
        //        // Skip opponent's turn and allow player 2 to take another action
        //        player1Script.isStunned = false;
        //        player1AttackButton.interactable = false;
        //        player1HealButton.interactable = false;
        //        player1DefendButton.interactable = false;
        //        player1ChargeButton.interactable = false;

        //        player2AttackButton.interactable = true;
        //        player2HealButton.interactable = true;
        //        player2DefendButton.interactable = true;
        //        player2ChargeButton.interactable = player2TurnsSinceCharge >= 6;

        //        StartCoroutine(ShowActionText("Player 1 is stunned!", player1ActionText));
        //        StartCoroutine(ShowActionText(" ", player2ActionText));
        //    }
        //}

        if(Turn % 2 == 0)
        {
            playerList[0].canPlay = true;
            playerList[1].canPlay = false;
        }
        else
        {
            playerList[1].canPlay = true;
            playerList[0].canPlay = false;
        }

        foreach(PlayerManager player in playerList)
        {
            player.OnSwitchTurnSettings();
        }
    }





    void Update()
    {
        if (!isGameOver)
        {
            if (player1Script.HasLost())
            {
                UpdateHonorAfterBattle(winner: 2, loser: 1);
                ShowGameOverMessage(2);
                isGameOver = true;
            }
            else if (player2Script.HasLost())
            {
                UpdateHonorAfterBattle(winner: 1, loser: 2);
                ShowGameOverMessage(1);
                isGameOver = true;
            }
        }
    }


private void ShowGameOverMessage(int winnerPlayerNumber)
{
    gameOverText.text = $"Player {winnerPlayerNumber} Wins!";
    gameOverText.gameObject.SetActive(true);
}

private void UpdateHonorAfterBattle(int winner, int loser)
{
    playerHonor[winner - 1] += 1;
    playerHonor[loser - 1] -= 0.25f;
    UpdateHonorUI();
}

}



