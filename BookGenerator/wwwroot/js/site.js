document.addEventListener("DOMContentLoaded", function () {
    const toggle = document.getElementById("themeToggle");
    const body = document.body;

    // Lataa tallennettu tila
    const savedTheme = localStorage.getItem("theme");
    if (savedTheme === "dark-mode") {
        body.classList.add("dark-mode");
        toggle.checked = true;
    }

    toggle.addEventListener("change", () => {
        const isDark = toggle.checked;
        body.classList.toggle("dark-mode", isDark);
        localStorage.setItem("theme", isDark ? "dark-mode" : "");
    });
});
