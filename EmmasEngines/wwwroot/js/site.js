// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const collapsedClass = "nav--collapsed";
const lskey = "navCollapsed";
const tooltipClass = "active";

const nav = document.querySelector(".home_header");
const navBorder = nav.querySelector(".nav__border");
//const menu = nav.querySelector(".menu");
    
if (localStorage.getItem(lskey) === "true") {
    nav.classList.add(collapsedClass);
    nav.classList.add(tooltipClass);
}

navBorder.addEventListener("click", () => {
    nav.classList.toggle(collapsedClass);
    nav.classList.toggle(tooltipClass);
    localStorage.setItem(lskey, nav.classList.contains(collapsedClass));
});
