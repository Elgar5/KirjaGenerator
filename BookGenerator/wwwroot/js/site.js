document.addEventListener("DOMContentLoaded", function () {
    const loginModal = document.getElementById("loginModal");
    const registerModal = document.getElementById("registerModal");
    const loginButton = document.getElementById("loginButton");
    const closeButtons = document.querySelectorAll(".close");
    const showRegister = document.getElementById("showRegister");
    const showLogin = document.getElementById("showLogin");
    const themeToggle = document.getElementById("themeToggle"); // Tumma/Vaalea tila -painike
    const body = document.body;

    // 🔹 Kirjautumis- ja rekisteröinti-ikkunan hallinta
    loginButton.onclick = function () {
        loginModal.style.display = "block";
    };

    showRegister.onclick = function () {
        loginModal.style.display = "none";
        registerModal.style.display = "block";
    };

    showLogin.onclick = function () {
        registerModal.style.display = "none";
        loginModal.style.display = "block";
    };

    closeButtons.forEach(button => {
        button.onclick = function () {
            loginModal.style.display = "none";
            registerModal.style.display = "none";
        };
    });

    window.onclick = function (event) {
        if (event.target === loginModal) {
            loginModal.style.display = "none";
        }
        if (event.target === registerModal) {
            registerModal.style.display = "none";
        }
    };

    // 🔹 Tumma/Vaalea tila -vaihto ja tallennus localStorageen
    const savedTheme = localStorage.getItem("theme");
    if (savedTheme) {
        body.classList.add(savedTheme);
        updateThemeButton(savedTheme);
    }

    themeToggle.addEventListener("click", function () {
        if (body.classList.contains("dark-mode")) {
            body.classList.remove("dark-mode");
            localStorage.setItem("theme", "");
            updateThemeButton("");
        } else {
            body.classList.add("dark-mode");
            localStorage.setItem("theme", "dark-mode");
            updateThemeButton("dark-mode");
        }
    });

    // Päivittää painikkeen tekstin ja ikonin
    function updateThemeButton(theme) {
        if (theme === "dark-mode") {
            themeToggle.innerHTML = "☀️ Vaalea tila";
        } else {
            themeToggle.innerHTML = "🌙 Tumma tila";
        }
    }
});

// 🔹 Simppeli kirjautumis- ja rekisteröintifunktio (päivitetty placeholder)
function registerUser() {
    alert("Rekisteröinti on vielä kehitteillä!");
}

function loginUser() {
    alert("Kirjautuminen on vielä kehitteillä!");
}
