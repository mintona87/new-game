mergeInto(LibraryManager.library, {

    SetupXverseEnv: function () {

        window.bs64escape = function (string) {
            return string.replace(/\+/g, "-").replace(/\//g, "_").replace(/=/g, "");
        }

        window.bs64encode = function (data) {
            if (typeof data === "object") {
                data = JSON.stringify(data);
            }
            return window.bs64escape(btoa(data));
        }
    },

    ConnectWalletXverse: async function (message = "Ordinals") {
        const provider = window.BitcoinProvider;

        if (!provider) {
            alert('No Bitcoin Wallet installed.');
            return;
        }

        let bs64header = window.bs64encode({
            alg: "none",
            typ: "JWT"
        });

        let bs64payload = window.bs64encode({
            purposes: ['ordinals', 'payment'],
            message: "Ordinals",
            network: {
                type: 'Mainnet'
            },
        });

        let jwt = bs64header + "." + bs64payload + '.';
        console.log(provider, '-------------provider');
        console.log(window.BitcoinProvider, '-------------window.BitcoinProvider');
        const callResponse = await provider.connect(jwt);

         try {
            window.unityInstance.SendMessage("WalletIntegration", "ReceiveWalletAddress", String("xverse,," + JSON.stringify(callResponse)));
        } catch {
            unityInstance.SendMessage("WalletIntegration", "ReceiveWalletAddress", String("xverse,," + JSON.stringify(callResponse)));
        }
        
    },

    FetchOrdinals: async function (address) {
      address = UTF8ToString(address);
    console.log(address,'-0----------------------------------------address');
      var dataArray = await getOrdinals(address);

      window.unityInstance.SendMessage('WalletIntegration', 'ReceiveModifiedMetadata', String(JSON.stringify({"data":dataArray})));
    }
});
