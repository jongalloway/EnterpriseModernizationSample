(function (window) {
    if (!window.jQuery) {
        return;
    }

    window.jQuery(function ($) {
        $('.module h3').on('click', function () {
            $(this).nextAll().toggle();
        });

        $('.utility-nav a').on('focus mouseenter', function () {
            $('.utility-nav a').removeClass('active-link');
            $(this).addClass('active-link');
        });
    });
}(window));
