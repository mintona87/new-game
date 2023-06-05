mergeInto(LibraryManager.library, {

    ConnectWalletPhantom: function () {
      if (typeof window.solana !== 'undefined') {
        if (window.solana.isPhantom) {
          window.solana.connect().then((result) => {  
            console.log("Phantom--:", result.publicKey.toBase58());  
            const walletAddress = result.publicKey.toBase58();
            window.unityInstance.SendMessage('WalletIntegration', 'ReceiveWalletAddress', String("phantom," + walletAddress));
          }).catch((error) => {  
            console.error(error);  
          });
        }
      }
    },

    FetchSolNFTs: async function (address) {
      var walletToQuery = UTF8ToString(address);
      walletToQuery = 'E645TckHQnDcavVv92Etc6xSWQaq8zzPtPRGBheviRAk';
      var dataArray = await getSolNFTs(walletToQuery);


      window.unityInstance.SendMessage('WalletIntegration', 'ReceiveModifiedMetadata', String(JSON.stringify({"data":dataArray})));
    }

});
