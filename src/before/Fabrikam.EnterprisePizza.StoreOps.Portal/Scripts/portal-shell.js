(function (window, document) {
    function initializeLegacyShell() {
        var tipsToggle = document.getElementById("legacyTipsToggle");
        var tipsPanel = document.getElementById("legacyTips");
        var selectedStoreField = document.getElementById("SelectedStoreNumber");
        var selectedStoreStamp = document.getElementById("selectedStoreStamp");

        if (tipsToggle && tipsPanel) {
            tipsToggle.onclick = function () {
                tipsPanel.style.display = tipsPanel.style.display === "block" ? "none" : "block";
                return false;
            };
        }

        if (selectedStoreField && selectedStoreStamp && selectedStoreField.value) {
            selectedStoreStamp.innerHTML = selectedStoreField.value;
        }
    }

    if (window.jQuery) {
        window.jQuery(function () {
            initializeLegacyShell();
        });
        return;
    }

    if (document.addEventListener) {
        document.addEventListener("DOMContentLoaded", initializeLegacyShell);
        return;
    }

    window.attachEvent("onload", initializeLegacyShell);
})(window, document);
