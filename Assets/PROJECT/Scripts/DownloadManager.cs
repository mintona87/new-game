using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

struct DownloadPair
{
    public string url;
    public Action<Texture2D> bookedAction;
}
public class DownloadManager : MonoBehaviour
{
    public static DownloadManager instance;
    List<DownloadPair> bookedDownloadList = new List<DownloadPair>();
    public bool isDownloading = false;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
    public void BookDownload(string url, Action<Texture2D> finishedAction)
    {
        DownloadPair _newPair = new DownloadPair();
        _newPair.url = url;
        _newPair.bookedAction = finishedAction;
        bookedDownloadList.Add(_newPair);
        if(!isDownloading)
        {
            StartDownload();
        }
    }
    void StartDownload()
    {
        if(bookedDownloadList.Count > 0)
        {
            StartCoroutine(DownloadImage(bookedDownloadList[0].url, bookedDownloadList[0].bookedAction));
        }
    }
    void ContinueDownload()
    {
        if (bookedDownloadList.Count > 0)
        {
            StartCoroutine(DownloadImage(bookedDownloadList[0].url, bookedDownloadList[0].bookedAction));
        }
    }
    public void ClearBooks()
    {
        bookedDownloadList = new List<DownloadPair>();
    }
    public IEnumerator DownloadImage(string MediaUrl, Action<Texture2D> editTexture)
    {
        Debug.Log(MediaUrl);
        isDownloading = true;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        request.certificateHandler = new CertificateWhore();
        yield return request.SendWebRequest();
        
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            //Texture2D webTexture = ((DownloadHandlerTexture)request.downloadHandler).texture as Texture2D;
            GlobalData.instance.downloadedTextures[MediaUrl] = ((DownloadHandlerTexture)request.downloadHandler).texture;
            editTexture(((DownloadHandlerTexture)request.downloadHandler).texture);
        }
        isDownloading = false;
        if (bookedDownloadList.Count > 0)
        {
            bookedDownloadList.RemoveAt(0);
        }
        ContinueDownload();
    }
    public class CertificateWhore : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
