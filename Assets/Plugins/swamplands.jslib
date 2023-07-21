mergeInto(LibraryManager.library, {
    OpenNewPage: function (str) 
    {
        var url = UTF8ToString(str);
        window.open(url, '_blank');
    },
    FocusTabChangeListen: function() 
    {
        document.addEventListener('visibilitychange', function () 
        {
            if (document.visibilityState === 'visible') 
            {
                SendMessage("SwamplandsDepositUI", "OnWindowActivate");
                console.log("TRIGGER FOCUS");
            }
        });
    }

});

