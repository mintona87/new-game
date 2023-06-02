using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NFTCell : MonoBehaviour
{
    public NFTMEtadata cellData;
    public Image nftImage;
    public Sprite nftSprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void InitCell(NFTMEtadata _data)
    {
        cellData = _data;
        if (_data.rawImage)
        {
            if(_data.imageUrl != null)
            {
                LoadSprite(GlobalData.instance.GetTextureFromBase64(_data.imageUrl));
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(_data.imageUrl))
            {
                DownloadManager.instance.BookDownload(_data.imageUrl, LoadSprite);
            }
        }
        //nftImage.sprite = 
    }
    void LoadSprite(Texture2D loadedTexture)
    {
        nftSprite = GlobalData.instance.LoadSpriteFromTexture(loadedTexture);
        nftImage.sprite = nftSprite;
    }
    private void OnDestroy()
    {
        DownloadManager.instance.UnBookDownload(cellData.imageUrl);
    }
}
