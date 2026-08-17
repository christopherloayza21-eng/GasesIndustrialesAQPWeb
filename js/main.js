const menuButtons = document.querySelectorAll("[data-menu-toggle]");
const navLinks = document.querySelectorAll(".nav-menu a");

menuButtons.forEach((button) => {
  button.addEventListener("click", () => {
    const navbar = button.closest(".navbar");
    const isOpen = navbar.classList.toggle("nav-open");

    button.setAttribute("aria-expanded", isOpen);
  });
});

navLinks.forEach((link) => {
  link.addEventListener("click", () => {
    const navbar = link.closest(".navbar");

    if (!navbar) {
      return;
    }

    const button = navbar.querySelector("[data-menu-toggle]");

    navbar.classList.remove("nav-open");

    if (button) {
      button.setAttribute("aria-expanded", "false");
    }
  });
});
