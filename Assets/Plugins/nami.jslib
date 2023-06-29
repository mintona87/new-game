mergeInto(LibraryManager.library, {

    ConnectWalletNami: function () {
      if (typeof window.cardano !== 'undefined') {
        if (typeof window.cardano.nami !== 'undefined') {
          window.cardano.nami.enable().then(function(api) {
            api.getUsedAddresses().then(function(addrs) {
              if (addrs[0]) {
                var base = "01"+addrs[0].slice(2);
                window.unityInstance.SendMessage('WalletIntegration', 'ReceiveWalletAddress', String("nami,," + base));
              } else {
                console.log('no used address');
              }
            })
          })
        }
      }
    }

});
