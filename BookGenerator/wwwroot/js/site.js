document.addEventListener("DOMContentLoaded", function () {
    const themeToggle = document.getElementById("themeToggle");
    const body = document.body;

    // 🔹 Ladataan tallennettu teema localStoragesta
    const savedTheme = localStorage.getItem("theme");
    if (savedTheme) {
        body.classList.add(savedTheme);
        updateThemeButton(savedTheme);
    }

    // 🔹 Tumma/Vaalea tila -napin toiminta
    themeToggle.addEventListener("click", function () {
        const isDark = body.classList.toggle("dark-mode");
        localStorage.setItem("theme", isDark ? "dark-mode" : "");
        updateThemeButton(isDark ? "dark-mode" : "");
    });

    // 🔹 Päivittää napin tekstin
    function updateThemeButton(theme) {
        themeToggle.innerHTML = theme === "dark-mode" ? "☀️ Vaalea tila" : "🌙 Tumma tila";
    }
});
