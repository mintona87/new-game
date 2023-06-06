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

    PlayfabManager playfabManager;
    LaunchManager launchManager;

    // Start is called before the first frame update
    void Start()
    {
        playfabManager = FindObjectOfType<PlayfabManager>();
        launchManager = FindObjectOfType<LaunchManager>();
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

        Debug.Log("localhonorAfter " + launchManager.GetCustomHonor() + " nfthonor " + honor);

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
        GlobalData.instance.SaveSelectedNFTData(selectedNFTData);
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
