(function (window, document) {
    function hasClass(element, className) {
        return (' ' + element.className + ' ').indexOf(' ' + className + ' ') > -1;
    }

    function addClass(element, className) {
        if (!hasClass(element, className)) {
            element.className = element.className ? element.className + ' ' + className : className;
        }
    }

    function removeClass(element, className) {
        var paddedClassName = ' ' + element.className + ' ';
        element.className = paddedClassName.replace(' ' + className + ' ', ' ').replace(/^\s+|\s+$/g, '');
    }

    function setActiveLink(activeLink) {
        var navLinks = document.querySelectorAll('.utility-nav a');
        var i;

        for (i = 0; i < navLinks.length; i += 1) {
            removeClass(navLinks[i], 'active-link');
        }

        addClass(activeLink, 'active-link');
    }

    function wireModuleToggles() {
        var moduleHeaders = document.querySelectorAll('.module h3');
        var i;

        for (i = 0; i < moduleHeaders.length; i += 1) {
            moduleHeaders[i].onclick = function () {
                var sibling = this.nextSibling;

                while (sibling) {
                    if (sibling.nodeType === 1) {
                        sibling.style.display = sibling.style.display === 'none' ? '' : 'none';
                    }

                    sibling = sibling.nextSibling;
                }
            };
        }
    }

    function wireUtilityNav() {
        var navLinks = document.querySelectorAll('.utility-nav a');
        var currentPath = window.location.pathname.toLowerCase();
        var i;

        for (i = 0; i < navLinks.length; i += 1) {
            if (navLinks[i].pathname && currentPath === navLinks[i].pathname.toLowerCase()) {
                setActiveLink(navLinks[i]);
            }

            navLinks[i].onfocus = function () {
                setActiveLink(this);
            };

            navLinks[i].onmouseenter = function () {
                setActiveLink(this);
            };
        }
    }

    window.onload = function () {
        wireModuleToggles();
        wireUtilityNav();
    };
}(window, document));
