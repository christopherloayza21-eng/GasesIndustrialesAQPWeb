const menuButtons = document.querySelectorAll("[data-menu-toggle]");

menuButtons.forEach((button) => {
  button.addEventListener("click", () => {
    const navbar = button.closest(".navbar");
    const isOpen = navbar.classList.toggle("nav-open");

    button.setAttribute("aria-expanded", isOpen);
  });
});
