mergeInto(LibraryManager.library, {

    JsonParse: function (metadatas) {

      const splitJson = UTF8ToString(metadatas).split('|||');

      // Initialize array to hold rearranged JSON objects
      const dataArray = [];

      for (var i=0; i<splitJson.length; i++) {
        var splitString = splitJson[i];
        if (!splitString) continue;

        // Split the string into name and data parts
        const [unit, dataString] = splitString.split('!@#');
        // Parse the data JSON string and extract desired properties
        const dataJson = JSON.parse(dataString);
        const policyId = Object.keys(Object.values(dataJson)[0])[0];
        const hexEncodedName = unit.split(policyId)[1];
        const name = Hex2a(hexEncodedName);


        const propertyData = dataJson["721"][policyId][name];

        // Rearrange extracted properties into desired format
        const properties = Object.entries(propertyData)
          .filter(([key]) => key !== 'Website' && key !== 'image')
          .map(([key, value]) => `${key.replace(/_/g, '-')}:::${value}`)
          .join('|||');

        dataArray.push({
          unit,
          name,
          imageUrl: propertyData.image.replace("ipfs://", "https://ipfs.io/ipfs/"),
          property: properties,
          website: propertyData.Website.trim(),
          hexEncodedName,
        });
      }
      console.log(dataArray, '----------dataArray');

      window.unityInstance.SendMessage('WalletIntegration', 'ReceiveModifiedMetadata', String(JSON.stringify({"data":dataArray})));
    },
});
