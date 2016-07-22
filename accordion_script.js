var acc = document.getElementsByClassName("accordion");
var i;

// Change this so we just register a function called expand() and apply it to each button?
// Just get everything working first and then refactor

for (i = 0; i < acc.length; i++) {
    acc[i].onclick = function () {
        this.classList.toggle("active");
        this.nextElementSibling.classList.toggle("show");
    }
}