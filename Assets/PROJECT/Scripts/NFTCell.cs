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
        DownloadManager.instance.BookDownload(_data.imageUrl, LoadSprite);
        //nftImage.sprite = 
    }
    void LoadSprite(Texture2D loadedTexture)
    {
        nftSprite = GlobalData.instance.LoadSpriteFromTexture(loadedTexture);
        nftImage.sprite = nftSprite;
    }
}
