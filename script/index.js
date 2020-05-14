function convertRemToPixels(rem) {
    return rem * parseFloat(getComputedStyle(document.documentElement).fontSize);
}

function putFooterAtBottom() {
    try {
        var viewHeight = window.innerHeight;
        var navHeight = document.querySelector("nav").offsetHeight;
        var mainHeight = document.querySelector("main").offsetHeight;
        var mainMargin = convertRemToPixels(0.5);

        var footer = document.querySelector("footer");
        var footerHeight = footer.offsetHeight;
        var viewExcludeFooterHeight = viewHeight - navHeight - mainHeight - 2 * mainMargin;
        if (viewExcludeFooterHeight > footerHeight) {
            if (!footer.classList.contains("fixed-bottom")) {
                footer.classList.add("fixed-bottom");
            }
        } else {
            if (footer.classList.contains("fixed-bottom")) {
                footer.classList.remove("fixed-bottom");
            }
        }
    } catch (e) {
        console.log(e);
    }
}

document.addEventListener("DOMContentLoaded",
    function () {
        putFooterAtBottom();

        galite("create", "UA-97849167-1", "auto");
        galite("send", "pageview");
    },
    false);

window.addEventListener("resize", function() {
    putFooterAtBottom();
});