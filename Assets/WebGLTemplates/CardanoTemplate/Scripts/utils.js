function Hex2a(hexx) {
    if (!hexx) return "";
    
    var hex = hexx.toString();//force conversion
    var str = '';
    for (var i = 0; i < hex.length; i += 2)
        str += String.fromCharCode(parseInt(hex.substr(i, 2), 16));
    return str;
}

function base64DecodeObject(obj) {
    if (!obj || typeof obj !== 'object') {
        return obj;
    }
    const decodedObj = {};
    for (const key in obj) {
        let decodedKey;
            decodedKey = atob(key.replaceAll('"', ''));
        let decodedValue;
        if (typeof obj[key] === 'string') {
            try {
            decodedValue = atob(obj[key]);
            } catch (e) {
            decodedValue = obj[key];
            }
        } else if (Array.isArray(obj[key])) {
            decodedValue = obj[key].map(i=>base64DecodeObject(i));
        } else {
            decodedValue = base64DecodeObject(obj[key]);
        }
        decodedObj[decodedKey] = decodedValue;
    }
    return decodedObj;
}

function getProperties(obj) {
    if (obj == null || obj == undefined || Object.keys(obj).length === 0) return;
console.log(obj)
    return Object.entries(obj)
    .filter(([key]) => key !== 'Website' && key !== 'image' && key !== 'description' && key !== 'file' && key !== 'files' && key !== 'name')
    .map(([key, value]) => getTerminalKeyValues(Object.fromEntries(new Map([[key, value]]))))
    .join('|||');
}

function getTerminalKeyValues(obj) {
    let str = '';
  
    function extractObject(obj, parentKey = '', arraySpec = '') {
        Object.keys(obj).forEach((key, index) => {
            const newKey = parentKey ? `${parentKey}.${key}` : key;
            const spec = arraySpec ? `${arraySpec}.${index + 1}` : `${index + 1}`;
    
            if (typeof obj[key] === 'object' && obj[key] !== null) {
            if (Array.isArray(obj[key])) {
                obj[key].forEach((item, i) => {
                extractObject(item, newKey, `${spec}_spec`);
                });
            } else {
                extractObject(obj[key], newKey);
            }
            } else {
            str += `${newKey}:::${obj[key]}|||`;
            }
        });
    }
  
    extractObject(obj);
    return str.slice(0, -3);
}

function getImageData(image) {
    var imageUrl="", rawImage=false;

    if ( image.constructor===Array ) {
      var fullImageUrl = image.join("");
      var strArray = fullImageUrl.split(",");
      imageUrl = strArray[1];
      rawImage = true;
    } else {
        if (image) {
            if ( image.includes("base64") || image.includes("data:image") ) {
                var strArray = imageUrl.split(",");
                imageUrl = strArray[1];
                rawImage = true;
            } else {
                imageUrl = image.replace("ipfs://ipfs/", "https://ipfs.io/ipfs/");
                imageUrl = imageUrl.replace("ipfs://", "https://ipfs.io/ipfs/");
            }
        } else {
            imageUrl = "";
        }
    }

    return {imageUrl, rawImage};
}

function getDescription(des) {
    var description="";
    if (des) {
        if ( des.constructor===Array ) {
            description = des.join("\n");
        } else {
            description = des? des:"";
        }
    }
    return description;
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function getSolNFTs() {
    const rpcEndpoint = 'https://solana-mainnet.rpc.extrnode.com';
    const solanaConnection = new solanaWeb3.Connection(rpcEndpoint);
    const TOKEN_PROGRAM_ID = new solanaWeb3.PublicKey('TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA');
    const { metadata: { Metadata } } = metaplex.programs
   
    const nfts = [];
    
    const tokenAccounts = await solanaConnection.getParsedTokenAccountsByOwner( 
        window.solana.publicKey, {
        programId: TOKEN_PROGRAM_ID
    });

    for (let index = 0; index < tokenAccounts.value.length; index++) {
        try{
            const tokenAccount = tokenAccounts.value[index];
            const tokenAmount = tokenAccount.account.data.parsed.info.tokenAmount;

            if (tokenAmount.amount == "1" && tokenAmount.decimals == 0) {
                let nftMint = new solanaWeb3.PublicKey(tokenAccount.account.data.parsed.info.mint)
                let tokenmetaPubKey = await Metadata.getPDA(nftMint);
                const metadata = await Metadata.load(solanaConnection, tokenmetaPubKey);
    console.log(metadata);
                
                const { data } = await axios.get(metadata.data.data.uri)
    console.log(data);
                nfts.push(data)
            }
        } catch(err) {
            continue;
        }
    }
    console.log(nfts);

    var dataArray = [];
    for (var i=0; i<nfts.length; i++) {
        var nft = nfts[i];

        var properties = getProperties(nft.attributes);
        var imageData = nft.image? getImageData(nft.image):"";
        
        var unit = nft.name;
        var name = nft.name;
        var description = nft.description? getDescription(nft.description):"";
        var imageUrl = imageData.imageUrl;
        var rawImage = imageData.rawImage;
        var hexEncodedName = "";
        var website = "";
        var property = properties;
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
    return dataArray;
}
