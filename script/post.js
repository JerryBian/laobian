document.addEventListener("DOMContentLoaded",
    function() {
        hljs.initHighlightingOnLoad();
        anchors.options = {
            placement: 'right',
            visible: 'hover'
        };
        anchors.add('.post-content h3, .post-content h4');

        try {
            var modal = document.querySelector("#imgModal");
            if (!modal) {
                return;
            }

            var images = document.querySelectorAll(".post img");
            var modalImg = document.querySelector("#modal-content");
            var caption = document.querySelector("#modal-caption");
            images.forEach(function(img) {
                img.onclick = function() {
                    modal.style.display = "block";
                    modalImg.src = this.src;
                    caption.innerHTML = this.alt;
                    document.documentElement.style.overflow = "hidden";
                    document.body.scroll = "no";
                };
            });

            document.querySelector("#modal-close").onclick = function() {
                modal.style.display = "none";
                document.documentElement.style.overflow = "auto";
                document.body.scroll = "yes";
            };
        } catch (e) {
            console.log(e);
        }
    },
    false);