﻿var acc = document.getElementsByClassName("accordion");
var i;

for (i = 0; i < acc.length; i++) {
    acc[i].onclick = function() {
        classList.toggle("active");
        nextElementSibling.classList.toggle("show");
    }
}