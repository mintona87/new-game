mergeInto(LibraryManager.library, {

    ConnectWalletMetamask: function () {
      if (typeof window.ethereum !== 'undefined') {
        if (window.ethereum.isMetaMask) {
          window.ethereum.request({ method: 'eth_requestAccounts' })
            .then(function(accounts) {
              if (accounts.length === 0) {
                console.log('Please connect to MetaMask.');
              } else { 
                window.unityInstance.SendMessage('WalletIntegration', 'ReceiveWalletAddress', String("metamask," + accounts[0]));
              }
            })
            .catch(function(err) {
              console.error(err);
            });
        }
      }
    },

    Fetch0xNFTs: async function (ownerAddr) {
      var requestOptions = {
          method: 'GET',
          redirect: 'follow'
      };

      var apiKey = "nCOwNKiqGDlELwoPRQEuZsU5n-e_iP6b";
      var baseURL = `https://eth-mainnet.g.alchemy.com/nft/v2/${apiKey}/getNFTs/`;
      
      var pageKey = "";
      ownerAddr = UTF8ToString(ownerAddr);

      var maxRetries = 10; 
      var retries = 0; 
      
      
      var nfts = [];

      
      for (;;) {
          var fetchURL = pageKey!=""? `${baseURL}?owner=${ownerAddr}&pageKey=${pageKey}` : `${baseURL}?owner=${ownerAddr}`

          var res = await fetch(fetchURL, requestOptions);
          
          if (res.status === 429 && retries < maxRetries) {
              var retryAfter = res.headers.get("Retry-After"); 
              if (retryAfter) {
                  var retryAfterMs = parseInt(retryAfter) * 1000; 
                  console.log(`Received 429 response, retrying after ${retryAfter} seconds`);
                  retries++;
                  await sleep(retryAfterMs);
              } else {
                  var retryAfterMs = Math.floor(Math.random() * 251) + 1000; 
                  console.log(`Received 429 response, retrying after ${retryAfterMs} ms`);
                  retries++;
                  await sleep(retryAfterMs);
              }
          } else if (res.ok) {
              var data = await res.json();
              nfts = nfts.concat(data.ownedNfts);
              pageKey = data.pageKey;
              if (data.ownedNfts.length < 100) break;
          } else {
            console.log(`Received ${res.status} status code`); 
            break;
          }

      }
      
      var dataArray = [];
      for (var i=0; i<nfts.length; i++) {
          var nft = nfts[i];
          var media = nft.media[0];
          var thumb = media.thumbnail;
          var mediaProperties = getProperties(mediaProperties);

          var propertyData = nft.metadata;
          var properties = getProperties(propertyData);
          var imageData = nft.metadata.image? getImageData(propertyData.image):"";
          
          var unit = nft.contract.address +"-"+ propertyData.id;
          var name = propertyData.name;
          var description = propertyData.description? getDescription(propertyData.description):"";
          var imageUrl = thumb? thumb:imageData.imageUrl;
          var rawImage = thumb? false:imageData.rawImage;
          var hexEncodedName = "";
          var website = "";
          var property = mediaProperties +"|||"+ properties;
          dataArray.push({
              unit: unit? unit:"",
              name: name? name:"",
              description: description? description:"",
              rawImage: rawImage? rawImage:"",
              imageUrl: imageUrl? imageUrl:"",
              hexEncodedName: hexEncodedName? hexEncodedName:"",
              website: website? website:"",
              property: property? property:"",
          });
      }
      console.log(dataArray)
      window.unityInstance.SendMessage('WalletIntegration', 'ReceiveModifiedMetadata', String(JSON.stringify({"data":dataArray})));
    }

});
