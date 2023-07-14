mergeInto(LibraryManager.library, {
    OpenNewPage: function (str) 
    {
        var url = UTF8ToString(str);
        window.open(url, '_blank');
    }
});