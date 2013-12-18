require([
        'bforms-namespace',
        'main-script',
        'bforms-panel',
        'bforms-fileupload'
], function () {

    $('.js-avatarContainer').bsFileUpload({
        url: requireConfig.pageOptions.uploadUrl,
        deleteUrl: requireConfig.pageOptions.deleteAvatarUrl
    });

    $('.bs-userInfo').bsPanel({
        name: 'userInfo'
    });

    $('.bs-contact').bsPanel({
        name: 'contact'
    });

});