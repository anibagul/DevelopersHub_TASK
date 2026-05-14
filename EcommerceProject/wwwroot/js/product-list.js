document.addEventListener("DOMContentLoaded", function () {
    const productContent = document.querySelector(".product-content");
    const gridViewBtn = document.getElementById("gridViewBtn");
    const listViewBtn = document.getElementById("listViewBtn");

    if (!productContent || !gridViewBtn || !listViewBtn) {
        return;
    }

    gridViewBtn.addEventListener("click", function () {
        productContent.classList.remove("list-mode");

        gridViewBtn.classList.add("active");
        listViewBtn.classList.remove("active");
    });

    listViewBtn.addEventListener("click", function () {
        productContent.classList.add("list-mode");

        listViewBtn.classList.add("active");
        gridViewBtn.classList.remove("active");
    });
});