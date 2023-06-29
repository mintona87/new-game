using System;
using System.Linq;
using System.Runtime.InteropServices;

using UnityEngine;

using Blockfrost;
using CardanoBech32;
using PeterO.Cbor2;
using System.Threading.Tasks;
using System.Text;

public class WalletConnect : MonoBehaviour
{
    
    public Configuration myConfiguration;
    public CBORObject cbor;
    private Cardano client;
    public GameObject loadingScreen;

    [DllImport("__Internal")]
    private static extern void ConnectWalletNami();

    [DllImport("__Internal")]
    private static extern void ConnectWalletEternl();

    [DllImport("__Internal")]
    private static extern void ConnectWalletMetamask();

    [DllImport("__Internal")]
    private static extern void ConnectWalletPhantom();

    [DllImport("__Internal")]
    private static extern void JsonParse(string metadatas);

    [DllImport("__Internal")]
    private static extern void Fetch0xNFTs(string address);

    [DllImport("__Internal")]
    private static extern void FetchSolNFTs(string address);

    [DllImport("__Internal")]
    private static extern void SetupXverseEnv();

    [DllImport("__Internal")]
    private static extern void ConnectWalletXverse();

    [DllImport("__Internal")]
    private static extern void FetchOrdinals(string address);

    public string Address { get; protected set; }
    public string NFT { get; protected set; }

    // Start is called before the first frame update
    void Start()
    {
        loadingScreen.SetActive(false);
        client = new Blockfrost.Cardano(myConfiguration);
    }

    public void OnClickNamiWalletConnect()
    {
#if UNITY_WEBGL
        ConnectWalletNami();
#else
                Debug.Log("WebGL not support");
#endif
        Debug.Log("waiting connect to nami..");
    }

    public void OnClickEternlWalletConnect()
    {
#if UNITY_WEBGL
        ConnectWalletEternl();
#else
        Debug.Log("WebGL not support");
#endif
        Debug.Log("waiting connect to eternl..");
    }

    public void OnClickMetamaskWalletConnect()
    {
#if UNITY_WEBGL
        ConnectWalletMetamask();
#else
        Debug.Log("WebGL not support");
#endif
        Debug.Log("waiting connect to metamask..");
    }

    public void OnClickPhantomWalletConnect()
    {
#if UNITY_WEBGL
        ConnectWalletPhantom();
#else
        Debug.Log("WebGL not support");
#endif
        Debug.Log("waiting connect to phantom..");
    }
    public void OnClickXverseWalletConnect()
    {
#if UNITY_WEBGL
        SetupXverseEnv();
        ConnectWalletXverse();

        //string arg = "xverse,{\"addresses\":[{\"address\":\"bc1p5mq9k9345lwa38ya064vgvdjnjsyzmwu38hew05wmksk5lc56gtqnl3dx3\",\"publicKey\":\"051fdc7fa6340f58cc72556b644eb7654f343221b9c4b1b0dd66fc950833623e\",\"purpose\":\"ordinals\"},{\"address\":\"3KZjiEmVFgYExuKe9r3RrNu2gUtvBViPaQ\",\"publicKey\":\"03a713798d3b85882e12e757d9f117e9d2a51ef5bc0c5be507825655fc09e01965\",\"purpose\":\"payment\"}]}";
        //string[] args = arg.Split(",");
        //string type = args[0];
        //string address = args[1];
        //Debug.Log("type:" + type);
        //Debug.Log("address:" + address);
        //var walletAddresses = JsonUtility.FromJson<Addresses>(address);
        //Debug.Log("Ordinals Wallet:" + walletAddresses.addresses[0].address);
        //Debug.Log("Payments Wallet:" + walletAddresses.addresses[1].address);
        //Address = walletAddresses.addresses[0].address;
#else
        Debug.Log("WebGL not support");
#endif
        Debug.Log("waiting connect to xverse..");
    }
    public void OnClickDisconnect()
    {
        LaunchManager.Instance.playfabManager.SelectedNftImageURL = "";
        LaunchManager.Instance.mainMenuController.SetPlayerSprite();
        GlobalData.instance.SetWalletInfo();
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
                //if (IsMatch(NFT, amount.unit))
                if (amount.quantity == "1")
                {
                    var nft = await client.GetSpecificAsset(amount.unit);
                    Debug.Log($"({nft})");
                    //await Task.Delay(1000);

                    if (IsMatch("000de140", nft.asset_name))
                    {
                        var refAsset = nft.policy_id + "000643b0" + nft.asset_name.Substring(8);
                        var refNft = await client.GetSpecificAsset(refAsset);
                        TxContentUtxo txContentUtxo = await client.GetTransactionUTXOs(refNft.initial_mint_tx_hash);
                        foreach (var output in txContentUtxo.outputs)
                        {
                            foreach (var refAmount in output.amount)
                            {
                                if (refAmount.unit == refAsset)
                                {
                                    //var data = HexStringToString(output.inline_datum);
                                    if (!String.IsNullOrEmpty(output.inline_datum))
                                    {
                                        byte[] bytes1 = StringToByteArray(output.inline_datum);
                                        cbor = CBORObject.DecodeFromBytes(bytes1);
                                        var metadata = cbor.ToJSONString();
                                        Debug.Log($"({metadata})");
                                        metadatas = metadatas + "|||" + amount.unit + "!@#" + nft.asset_name + "!@#" + metadata + "!@#updated";
                                    }
                                    else if (!String.IsNullOrEmpty(output.data_hash))
                                    {
                                        ScriptDatumCbor data = await client.GetDatumCBORValue(output.data_hash);
                                        byte[] bytes = StringToByteArray(data.cbor);
                                        cbor = CBORObject.DecodeFromBytes(bytes);
                                        var metadata = cbor.ToJSONString();
                                        Debug.Log($"({metadata})");
                                        metadatas = metadatas + "|||" + amount.unit + "!@#" + nft.asset_name + "!@#" + metadata + "!@#updated";
                                    }
                                    else
                                    {
                                        var txContentMetadataCbor = await client.GetTransactionMetadataInCBOR(nft.initial_mint_tx_hash);
                                        //await Task.Delay(1000);
                                        Debug.Log($"({txContentMetadataCbor})");

                                        //await Task.Delay(1000);
                                        if (txContentMetadataCbor.Length != 0)
                                        {
                                            byte[] bytes1 = StringToByteArray(txContentMetadataCbor[0].metadata);
                                            cbor = CBORObject.DecodeFromBytes(bytes1);
                                            var metadata = cbor.ToJSONString();
                                            Debug.Log($"({metadata})");
                                            metadatas = metadatas + "|||" + amount.unit + "!@#" + nft.asset_name + "!@#" + metadata;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var txContentMetadataCbor = await client.GetTransactionMetadataInCBOR(nft.initial_mint_tx_hash);
                        //await Task.Delay(1000);
                        Debug.Log($"({txContentMetadataCbor})");

                        //await Task.Delay(1000);
                        if (txContentMetadataCbor.Length != 0)
                        {
                            byte[] bytes1 = StringToByteArray(txContentMetadataCbor[0].metadata);
                            cbor = CBORObject.DecodeFromBytes(bytes1);
                            var metadata = cbor.ToJSONString();
                            Debug.Log($"({metadata})");
                                metadatas = metadatas + "|||" + amount.unit + "!@#" + nft.asset_name + "!@#" + metadata;
                        }
                    }
                }
            }
            Debug.Log($"{metadatas}");
            JsonParse(metadatas);
        }
        catch (Exception e)
        {
            //  Block of code to handle errors
            Debug.Log("No nft"+ e);
            loadingScreen.SetActive(false);
            return;
        }

    }

    public void ReceiveWalletAddress(string arg)
    {
        string[] args = arg.Split(",,");
        string type = args[0];
        string address = args[1];

        loadingScreen.SetActive(true);
        Debug.Log(arg);
        if (type == "nami" || type == "eternl")
        {
            var converter = new CardanoBech32Wrapper();
            Address = converter.ConvertToBech32AddressFromHex(address, AddressType.addr);

            //Address = "addr1q95vldfjvqd5vuxe6e2pxee5rl9l0tj5949dkzvg8t2dymdrmmz5crk4sm24rgppdmy9ytfn2f0cv9q9e3r59443qd2salsw7w";
            GlobalData.instance.SetWalletInfo(true, type, Address);
            FetchNFTs();
        }
        else if (type == "metamask")
        {
            Address = address;
            GlobalData.instance.SetWalletInfo(true, type, Address);
            Fetch0xNFTs(Address);
        }
        else if (type == "phantom")
        {
            Address = address;
            GlobalData.instance.SetWalletInfo(true, type, Address);
            FetchSolNFTs(Address);
        }
        else if (type == "xverse")
        {

            try
            {
                var walletAddresses = JsonUtility.FromJson<Addresses>(address);
                Debug.Log("Ordinals Wallet:" + walletAddresses.addresses[0].address);
                Debug.Log("Payments Wallet:" + walletAddresses.addresses[1].address);
                Address = walletAddresses.addresses[0].address;
                GlobalData.instance.SetWalletInfo(true, type, walletAddresses.addresses[0].address);
                FetchOrdinals(Address);
            }
            catch (Exception e)
            {
                Debug.LogError("Error while parsing addresses from wallet.");
                throw;
            }
        }

        Debug.Log(Address);

    }

    public void ReceiveModifiedMetadata(string data)
    {
        Debug.Log(data);
        loadingScreen.SetActive(false);
        GlobalData.instance.LoadNFTMetaData(data);
    }









    public static bool IsMatch(string header, string body)
    {
        if (header.Length > body.Length)
        {
            return false;
        }

        for (int i = 0; i < header.Length; i++)
        {
            if (header[i] != body[i])
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

    public static string StringToHexString(string str)
    {
        byte[] bytes = Encoding.Default.GetBytes(str);
        
        string hexString = BitConverter.ToString(bytes);
        hexString = hexString.Replace("-", "");
        return hexString;
    }

    public static string HexStringToString(string hex)
    {
        byte[] dBytes = StringToByteArray(hex);


        string ASCIIresult = System.Text.Encoding.ASCII.GetString(dBytes);

        //To get the UTF8 value of the hex string
        string utf8result = System.Text.Encoding.UTF8.GetString(dBytes);
        return utf8result;
    }
}
