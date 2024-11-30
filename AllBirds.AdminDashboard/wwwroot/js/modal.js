// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function openModal(deleteType, urlToGo) {
    //let parent = evt.target.children.length ? evt.target.parentElement : evt.target.parentElement.parentElement;
    let modal = window.document.getElementById("deleteModal");
    modal.style = "display: block; opacity: 1;"
    let modal_msg = window.document.getElementById("modal-msg");
    modal_msg.textContent = "Are You Sure You Want To Delete This " + deleteType;
    let deleteBtn = window.document.getElementById("modal-delete-btn");
    deleteBtn.href = urlToGo;
    console.log(deleteType, urlToGo, modal);
    //parent.setAttribute("style", "display:none;");
}

function closeModal() {
    let modal = window.document.getElementById("deleteModal");
    modal.style = "display: none; opacity: 0;"
}