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
        name: 'userInfo',
        editSuccessHandler : function(e, data) {
            $('.js-userName').text(data.Username);
        }
    });

$('.bs-contact').bsPanel({
    name: 'contact',
});

});