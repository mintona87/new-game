1. I created the DepositSwamplandsManager script
which controls the UI elements and allows you to run CloudScript execution on PlayFab.
In it, before executing the logic, it is necessary to initialize the value for _playerPlayFabID, that is, log in through PlayFabApi as a player. I take PlayFabID from GlobalData.instance?.playfabId for now.

-> Prefab - SwamplandsDepositUI - implements a simple UI to manage all the connection and deposit logic of C4. The deposit is credited to the balance of the virtual currency GOLD in PlayFab.

2. Button (1)Connect swamplands will only work in webgl build, as it uses swamplands.jslib to run js method which will allow you to open page in new vault to login to swamplands. To log in to the editor, you must manually open the page
https://swamplands.cardanocrocsclub.com/login-with-swamplands/PFPPB?user={userPlayFabID}
userPlayFabID - Player ID on PlayFab
and log in.

3. swamplands.jslib - should be in the Assets/Plugins folder for it to work.

I will be happy to answer any additional questions for you.