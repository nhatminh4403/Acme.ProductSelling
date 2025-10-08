$(function () {

    var nameInput = $('#Blog_Title');

    var slugInput = $('#Blog_UrlSlug');

    if (nameInput.length && slugInput.length) {
        nameInput.on('input', function () {
            var nameValue = $(this).val();
            var slugValue = generateSlug(nameValue);
            slugInput.val(slugValue);
        });

        if (nameInput.val()) {
            slugInput.val(generateSlug(nameInput.val()));
        }
    }

    function generateSlug(text) {
        if (!text) {
            return "";
        }

        text = text.replace(/Đ/g, 'D');
        text = text.replace(/đ/g, 'd');


        text = text.normalize('NFD').replace(/[\u0300-\u036f]/g, '');


        text = text.replace(/\s+/g, '-');

        text = text.replace(/[^\w-]+/g, '');

        text = text.toLowerCase();

        text = text.replace(/-+/g, '-');
        text = text.replace(/^-+|-+$/g, '');

        return text;
    }
});