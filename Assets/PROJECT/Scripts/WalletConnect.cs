using System;
using System.Linq;
using System.Runtime.InteropServices;

using UnityEngine;

using Blockfrost;
using CardanoBech32;
using PeterO.Cbor2;
using System.Threading.Tasks;

public class WalletConnect : MonoBehaviour
{
    
    public Configuration myConfiguration;
    public CBORObject cbor;
    private Cardano client;


    [DllImport("__Internal")]
    private static extern void ConnectWalletNami();

    [DllImport("__Internal")]
    private static extern void ConnectWalletEternl();

    [DllImport("__Internal")]
    private static extern void JsonParse(string metadatas);

    public string Address { get; protected set; }
    public string NFT { get; protected set; }

    // Start is called before the first frame update
    void Start()
    {
        client = new Blockfrost.Cardano(myConfiguration);
        //Address = "addr1qy9v0eaay6j4ypnj7tpegqw6ftkdc57fh58pwdzfenrqecx7ghgwceed03tl3f8k2htmf3ut7mkt8a3jcg6szn7yh9hsmpksxz";
        NFT = "aa19d5f5ae9b6c93c8e278851194553ddd4789d77f86d3ad2f7480d8"; // Cardano Crocs Club
        //Address = "addr1q9cqfw8tmcmtrxtk8mlp259qeu2qslrh8mmuvjt276r34vgd60j6flje32l6py8jetyreqcghd6t3rsrnv7mahqkl07sdplgaq";
        //NFT = "0889a2d542897f0c7eefed47d2d809bd8d8ec78881bd4ff9464f683a"; // PFPPB
    }
    public void OnClickNamiWalletConnect()
    {
        // GlobalData.instance.LoadNFTMetaData();
#if UNITY_WEBGL
        ConnectWalletNami();
#else
        Debug.Log("WebGL not support");
#endif
        Debug.Log("waiting connect to nami..");
    }
    public void OnClickEernlWalletConnect()
    {
        // GlobalData.instance.LoadNFTMetaData();
#if UNITY_WEBGL
        ConnectWalletEternl();
#else
        Debug.Log("WebGL not support");
#endif
        Debug.Log("waiting connect to eternl..");
    }

    public async void FetchNFTs()
    {
        Debug.Log("hhh, m here");
        string metadatas = "";

        try 
        {
            var ac = await client.GetSpecificAddress(Address);
            Debug.Log($"{ac}");
            //await Task.Delay(1000);
            foreach (var amount in ac.amount)
            {
                Debug.Log($"({amount})");
                if (IsMatch(NFT, amount.unit))
                {
                    var nft = await client.GetSpecificAsset(amount.unit);
                    //await Task.Delay(1000);
                    var txContentMetadataCbor = await client.GetTransactionMetadataInCBOR(nft.initial_mint_tx_hash);
                    //await Task.Delay(1000);
                    byte[] bytes1 = StringToByteArray(txContentMetadataCbor[0].metadata);
                    cbor = CBORObject.DecodeFromBytes(bytes1);
                    var metadata = cbor.ToJSONString();
                    metadatas = metadatas + "|||" + amount.unit + "!@#" + metadata;

                }

            }
            Debug.Log($"{metadatas}");
            JsonParse((metadatas));
        }
        catch (Exception e)
        {
            //  Block of code to handle errors
            Debug.Log("No nft");
            return;
        }
        
    }

    public void ReceiveWalletAddress(string address)
    {
        Debug.Log(address);
        GlobalData.instance.isWalletConnected = true;
        var converter = new CardanoBech32Wrapper();
        Address = converter.ConvertToBech32AddressFromHex(address, AddressType.addr);
        Debug.Log(Address);
        //Address = "addr1qy9v0eaay6j4ypnj7tpegqw6ftkdc57fh58pwdzfenrqecx7ghgwceed03tl3f8k2htmf3ut7mkt8a3jcg6szn7yh9hsmpksxz";
        FetchNFTs();
    }

    public void ReceiveModifiedMetadata(string data)
    {
        Debug.Log(data);
        GlobalData.instance.LoadNFTMetaData(data);
    }









    public static bool IsMatch(string policyId, string asset)
    {
        if (policyId.Length > asset.Length)
        {
            return false;
        }

        for (int i = 0; i < policyId.Length; i++)
        {
            if (policyId[i] != asset[i])
            {
                return false;
            }
        }

        return true;
    }

    public static byte[] StringToByteArray(string hex) => Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
}
