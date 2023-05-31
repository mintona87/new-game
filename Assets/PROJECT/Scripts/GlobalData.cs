using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

[System.Serializable]
public class NFTMEtadata
{
    public string unit;
    public string name;
    public string description;
    public bool rawImage;
    public string imageUrl;
    public string hexEncodedName;
    public string website;
    public string property;
}
[System.Serializable]
public class NFTMEtadataList
{
    public NFTMEtadata[] data;
}
public struct NFTMetadataProperty
{
    public string key;
    public string value;
}
public class GlobalData : MonoBehaviour
{
    public static GlobalData instance;
    public NFTMEtadata[] nftDataList;
    public Dictionary<string, Texture2D> downloadedTextures = new Dictionary<string, Texture2D>();
    public bool isWalletConnected = false;
    public string playfabId;
    public Dictionary<string, string> playerData = new Dictionary<string, string>();

    public Action<NFTMEtadata> nftSelectAction = null;
    string jsonData = "{\"data\":[{\"unit\":\"123\",\"name\":\"456\",\"imageUrl\":\"https://ipfs.io/ipfs/QmXfB96Nm16Yf8eSnYUw69vx54iP64oVcyfS8BEKWv3CfN\",\"property\":\"zc-/-ewrwer///xv-/-werwer\"},{\"unit\":\"123\",\"name\":\"sefsdf\",\"imageUrl\":\"https://ipfs.io/ipfs/QmXfB96Nm16Yf8eSnYUw69vx54iP64oVcyfS8BEKWv3CfN\",\"property\":\"zsdfec-/-ewrwsefer///xv-/-werwer\"}]}";
    string splitStr = "|||";
    string subSplitStr = ":::";

    public Texture2D testTexture;
    
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    public void LoadNFTMetaData(string _jsonStr)
    {
        NFTMEtadataList nftData = JsonUtility.FromJson<NFTMEtadataList>(_jsonStr);
        nftDataList = nftData.data;
    }
    public List<NFTMetadataProperty> GetNFTProperties(string _propertyStr)
    {
        List<NFTMetadataProperty> _propertyList = new List<NFTMetadataProperty>();
        string[] propertyStrs = _propertyStr.Split(new string[] { splitStr }, System.StringSplitOptions.None);
        for(int i = 0; i < propertyStrs.Length; i++)
        {
            string[] subStrs = propertyStrs[i].Split(new string[] { subSplitStr }, System.StringSplitOptions.None);
            if(subStrs.Length >= 2)
            {
                NFTMetadataProperty _property = new NFTMetadataProperty();
                _property.key = subStrs[0];
                _property.value = subStrs[1];
                _propertyList.Add(_property);
            }
        }
        return _propertyList;
    }
    public Sprite LoadSpriteFromTexture(Texture2D tex)
    {
        Vector2 _pivot = new Vector2(0.5f, 0.5f);
        Rect _rect = new Rect(0, 0, tex.width, tex.height);
        Sprite _resultSpr = Sprite.Create(tex, _rect, _pivot);
        return _resultSpr;
    }
    public void SaveSelectedNFTData(NFTMEtadata _data)
    {
        playerData["SelectedNFT"] = JsonUtility.ToJson(_data);
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = playerData
        },
        result => {
            Debug.Log("Successfully updated user data");
            if(nftSelectAction != null)
            {
                nftSelectAction(_data);
            }
        },
        error => {
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public void GetUserData(string _playfabId)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = _playfabId,
            Keys = null
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("SelectedNFT")) Debug.Log("No SelectedNFT");
            else Debug.Log("SelectedNFT: " + result.Data["SelectedNFT"].Value);
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public Texture2D GetTextureFromBase64(string _base64Str)
    {
        byte[] imageBytes = Convert.FromBase64String(_base64Str);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageBytes);
        return tex;
    }
    public void ReduceTextureSize()
    {
        Debug.Log("texture size: " + testTexture.width.ToString());
        testTexture.Reinitialize(testTexture.width / 2, testTexture.height / 2, TextureFormat.RGBA32, false);
        testTexture.Apply();
        Debug.Log("texture size: " + testTexture.desiredMipmapLevel.ToString());
    }
}
