// Purpose: JavaScript for the website to enable dark mode or light mode

const themeToggleBtn = document.getElementById('themeToggle');

function setTheme(theme) {
    if (theme === 'dark') {
        document.body.classList.add('dark-mode');
        themeToggleBtn.textContent = '☀️ Vaalea tila';
    } else {
        document.body.classList.remove('dark-mode');
        themeToggleBtn.textContent = '🌙 Tumma tila';
    }
    localStorage.setItem('theme', theme);
}

themeToggleBtn.addEventListener('click', () => {
    const currentTheme = document.body.classList.contains('dark-mode') ? 'light' : 'dark';
    setTheme(currentTheme);
});

// Tarkista, onko käyttäjä aiemmin valinnut teeman
const savedTheme = localStorage.getItem('theme') || 'light';
setTheme(savedTheme);
