document.addEventListener("DOMContentLoaded", function () {
    const loginModal = document.getElementById("loginModal");
    const registerModal = document.getElementById("registerModal");
    const loginButton = document.getElementById("loginButton");
    const logoutButton = document.getElementById("logoutButton"); // Lisää logout-painike
    const closeButtons = document.querySelectorAll(".close");
    const showRegister = document.getElementById("showRegister");
    const showLogin = document.getElementById("showLogin");
    const themeToggle = document.getElementById("themeToggle"); // Tumma/Vaalea tila -painike
    const body = document.body;

    // 🔹 Kirjautumis- ja rekisteröinti-ikkunan hallinta
    if (loginButton) {
        loginButton.onclick = function () {
            loginModal.style.display = "block";
        };
    }

    if (showRegister) {
        showRegister.onclick = function () {
            loginModal.style.display = "none";
            registerModal.style.display = "block";
        };
    }

    if (showLogin) {
        showLogin.onclick = function () {
            registerModal.style.display = "none";
            loginModal.style.display = "block";
        };
    }

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

    if (themeToggle) {
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
    }

    // Päivittää painikkeen tekstin ja ikonin
    function updateThemeButton(theme) {
        if (theme === "dark-mode") {
            themeToggle.innerHTML = "☀️ Vaalea tila";
        } else {
            themeToggle.innerHTML = "🌙 Tumma tila";
        }
    }

    // 🔹 Päivitä navigaatiopalkki kirjautumistilan mukaan
    updateNavbar();
});

// 🔹 Rekisteröi käyttäjä SQL-tietokantaan
function registerUser() {
    const username = document.getElementById("register-username").value;
    const password = document.getElementById("register-password").value;

    fetch("/Account/Register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password })
    })
    .then(response => response.json())
    .then(data => {
        alert(data.message);
        if (data.success) {
            document.getElementById("registerModal").style.display = "none";
            document.getElementById("loginModal").style.display = "block";
        }
    })
    .catch(error => console.error("Error:", error));
}

// 🔹 Kirjaa käyttäjä sisään SQL-tietokannasta
function loginUser() {
    const username = document.getElementById("login-username").value;
    const password = document.getElementById("login-password").value;

    fetch("/Account/Login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password })
    })
    .then(response => response.json())
    .then(data => {
        alert(data.message);
        if (data.success) {
            document.getElementById("loginModal").style.display = "none";
            location.reload(); // Päivitetään sivu, jotta navbar näyttää kirjautuneen käyttäjän
        }
    })
    .catch(error => console.error("Error:", error));
}

// 🔹 Kirjaa käyttäjä ulos
function logoutUser() {
    fetch("/Account/Logout", { method: "POST" })
    .then(() => location.reload());
}

// 🔹 Päivittää navigaatiopalkin kirjautumistilan mukaan
function updateNavbar() {
    fetch("/Account/CheckSession")
    .then(response => response.json())
    .then(data => {
        const navRight = document.querySelector(".nav-right");
        if (data.isLoggedIn) {
            navRight.innerHTML = `
                <span>Welcome, ${data.username}</span>
                <button id="logoutButton">🚪 Logout</button>
                <button id="themeToggle">🌙 Tumma tila</button>
            `;
            document.getElementById("logoutButton").addEventListener("click", logoutUser);
        } else {
            navRight.innerHTML = `
                <button id="loginButton">🔑 Login</button>
                <button id="themeToggle">🌙 Tumma tila</button>
            `;
            document.getElementById("loginButton").addEventListener("click", function () {
                document.getElementById("loginModal").style.display = "block";
            });
        }
    })
    .catch(error => console.error("Error checking session:", error));
}
