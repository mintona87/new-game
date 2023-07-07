using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NFTPanel : MonoBehaviour
{
    public NFTCell nftCellPref;
    public Transform nftListContent;
    List<GameObject> istantiatedObjects = new List<GameObject>();
    NFTMEtadata selectedNFTData;

    public Image nftPotrait;
    public TMP_Text nftName;
    public TMP_Text nftDescription;
    public TMP_Text[] nftTraits;
    public TMP_Text walletAddress;
    public Image walletImage;
    public TMP_Text nftHonor;
    public TMP_Text nftAtk;
    public TMP_Text nftDef;
    public TMP_Text nftSpd;
    public TMP_Text nftLuck;
    public TMP_Text nftGold;


    PlayfabManager playfabManager;
    LaunchManager launchManager;
    PlayerManager playerManager;

    // Start is called before the first frame update
    void Start()
    {
        playfabManager = FindObjectOfType<PlayfabManager>();
        launchManager = FindObjectOfType<LaunchManager>();
        playerManager = FindObjectOfType<PlayerManager>();
    }
    private void OnEnable()
    {
        InitPanel();
    }
    public void InitPanel()
    {
        walletAddress.text = GlobalData.instance.connectedWallet.address;
        walletImage.sprite = Resources.Load<Sprite>("Assets/Resources/Sprites/" + GlobalData.instance.connectedWallet.type + "80");

        for (int i = 0; i < GlobalData.instance.nftDataList.Length; i++)
        {
            NFTCell _nftCell = Instantiate(nftCellPref, nftListContent) as NFTCell;
            istantiatedObjects.Add(_nftCell.gameObject);
            _nftCell.gameObject.SetActive(true);
            _nftCell.InitCell(GlobalData.instance.nftDataList[i]);
        }
    }
    private void OnDisable()
    {
        for(int i = 0; i < istantiatedObjects.Count; i ++)
        {
            Destroy(istantiatedObjects[i]);
        }
    }
    public void SelectNFTCell(NFTMEtadata _nftData, Sprite _sprite)
    {
        nftPotrait.sprite = _sprite;
        nftName.text = _nftData.name;
        nftDescription.text = _nftData.description;

        int honor = GlobalData.instance.GetNFTHonor(_nftData.unit);
        nftHonor.text = "Honor: " + honor.ToString();

        Debug.Log("ne localhonor " + launchManager.GetCustomHonor() + " nfthonor" + honor);

        launchManager.ModifyPlayerCustomHonor(honor);

        for (int i = 0; i < nftTraits.Length; i++)
        {
            nftTraits[i].gameObject.SetActive(false);
        }
        List<NFTMetadataProperty> properties = GlobalData.instance.GetNFTProperties(_nftData.property);
        for (int i = 0; i < properties.Count; i++)
        {
            if(i < nftTraits.Length)
            {
                nftTraits[i].gameObject.SetActive(true);
                nftTraits[i].text = properties[i].key + ": " + properties[i].value;
            }
        }

        selectedNFTData = _nftData;
    }
    public void OnClickNFTCell(NFTCell _cell)
    {
        SelectNFTCell(_cell.cellData, _cell.nftSprite);
    }
    public void OnClickNFTSelectBtn()
    {
        playfabManager.getSelectedNFTData = selectedNFTData;
        playfabManager.SelectedNftImageURL = selectedNFTData.imageUrl;
        launchManager.ModifyPlayerCustomImageURL(playfabManager.SelectedNftImageURL);
        GlobalData.instance.SaveSelectedNFTData(selectedNFTData);

        float atk = 1;
        float def = 1;
        float spd = 1;
        float luck = 1;
        float gold = 1;
        float honor = 1;
        if (selectedNFTData.unit != "" && selectedNFTData.unit != null)
        {
            string[] policyIds = selectedNFTData.unit.Split(new string[] { selectedNFTData.hexEncodedName }, System.StringSplitOptions.None);

            if (policyIds[0] == "25fef4794291774ee90ef721259460c2c00b655b718285e27ffa4ebe") // DRAGONs
            {
                List<NFTMetadataProperty> properties = GlobalData.instance.GetNFTProperties(selectedNFTData.property);
                for (int i = 0; i < properties.Count; i++)
                {
                    if (properties[i].key == "PFPPB ATK")
                    {
                        atk += float.Parse(properties[i].value);
                    }
                    else if (properties[i].key == "PFPPB DEF")
                    {
                        def += float.Parse(properties[i].value);
                    }
                    else if (properties[i].key == "PFPPB SPD")
                    {
                        spd += float.Parse(properties[i].value);
                    }
                    else if (properties[i].key == "PFPPB LUCK")
                    {
                        luck += float.Parse(properties[i].value);
                    }
                }
            }
        }
        Debug.Log("//////////////////////////////////////////////////" + atk + def + spd + luck);
        PlayerSavedData playerSavedData = new PlayerSavedData(
            atk,//ATT
            def,//DEF
            spd,//SPD
            luck,//LUCK
            0,//Gold
            1//Honor
        );

            // Update the UI elements with the new stats
            nftAtk.text = "Attack: " + atk.ToString();
            nftDef.text = "Defense: " + def.ToString();
            nftSpd.text = "Speed: " + spd.ToString();
            nftLuck.text = "Luck: " + luck.ToString();
            nftGold.text = "Gold: " + gold.ToString();
            nftHonor.text = "Honor: " + honor.ToString();

        playfabManager.SavePlayerSavedData(playerSavedData);
    }
    public void OnClickGetSavedNFTData()
    {
        GlobalData.instance.GetUserData(GlobalData.instance.playfabId);
    }
    public void DebugAddHonorAndSave()
    {

        if (playfabManager.getSelectedNFTData.unit != "")
        {
            int honor = GlobalData.instance.GetNFTHonor(playfabManager.getSelectedNFTData.unit) + 10;
            GlobalData.instance.SaveNFTHonor(playfabManager.getSelectedNFTData.unit, honor);

            Debug.Log("ne localhonor " + launchManager.GetCustomHonor() + " nfthonor" + honor);

            launchManager.ModifyPlayerCustomHonor(honor);
        }

    }
}
