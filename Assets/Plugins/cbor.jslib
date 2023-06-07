
mergeInto(LibraryManager.library, {

    JsonParse: async function (metadatas) {

      const splitJson = UTF8ToString(metadatas).split('|||');

      // Initialize array to hold rearranged JSON objects
      const dataArray = [];
      console.log(metadatas, splitJson, '----------metadatas, splitJson ');

      for (var i=0; i<splitJson.length; i++) {
        var splitString = splitJson[i];
        if (!splitString) continue;

        // Split the string into name and data parts
        const [unit, dataString, isUpdated] = splitString.split('!@#');
        // Parse the data JSON string and extract desired properties
        const dataJson = JSON.parse(dataString);
        
        var name;
        var imageData;
        var properties, hexEncodedName="", website=""
        var description;
        if (isUpdated) {
            const propertyData = base64DecodeObject(JSON.parse(dataString)[0]);

            name = propertyData.name? propertyData.name:"";
            properties = getProperties(propertyData);
            imageData = propertyData.image? await getImageData(propertyData.image):{imageUrl:"", rawImage:"", isInvalidImage:false};
            description = propertyData.description? getDescription(propertyData.description):"";
            
        } else {
    
            if (!dataJson["721"]) continue;
            
            const policyId = Object.keys(Object.values(dataJson)[0])[0];
            hexEncodedName = unit.split(policyId)[1];
    
            if (!dataJson || !policyId || !hexEncodedName) continue;
        
            name = Hex2a(hexEncodedName);
            const propertyData = dataJson["721"][policyId][name];
            properties = getProperties(propertyData);
            imageData = propertyData.image? await getImageData(propertyData.image):{imageUrl:"", rawImage:"", isInvalidImage:false};
            description = propertyData.description? getDescription(propertyData.description):"";
            website = propertyData.Website? propertyData.Website:"";
    
        }

        if (imageData.isInvalidImage) continue;

        dataArray.push({
          unit,
          name,
          imageUrl:imageData.imageUrl,
          rawImage:imageData.rawImage,
          property: properties,
          website,
          hexEncodedName,
          description,
        });
      }
      console.log(dataArray, '----------dataArray');

      window.unityInstance.SendMessage('WalletIntegration', 'ReceiveModifiedMetadata', String(JSON.stringify({"data":dataArray})));
    },
});
