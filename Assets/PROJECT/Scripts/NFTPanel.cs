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

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        InitPanel();
    }
    public void InitPanel()
    {
        for(int i = 0; i < GlobalData.instance.nftDataList.Length; i++)
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
        GlobalData.instance.SaveSelectedNFTData(selectedNFTData);
    }
    public void OnClickGetSavedNFTData()
    {
        GlobalData.instance.GetUserData(GlobalData.instance.playfabId);
    }
}
