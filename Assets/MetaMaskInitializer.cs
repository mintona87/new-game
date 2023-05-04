//using UnityEngine;
//using UnityEngine.UI;
//using MetaMask.Unity;
//using System;

//public class MetaMaskInitializer : MonoBehaviour
//{
//    public Button connectButton;

//    private void Start()
//    {
//        connectButton.onClick.AddListener(InitializeAndConnectMetaMask);
//    }

//    private void InitializeAndConnectMetaMask()
//    {
//        MetaMaskUnity.Instance.Initialize();
//        ConnectToWallet();
//    }

//    private void ConnectToWallet()
//    {
//        var wallet = MetaMaskUnity.Instance.Wallet;
//        wallet.WalletConnected += OnWalletConnected;
//        wallet.Connect();
//    }

//    private void OnWalletConnected(object sender, EventArgs e)
//    {
//        Debug.Log("Wallet is connected");
//    }
//}
