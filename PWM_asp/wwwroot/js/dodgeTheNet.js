function generatePassword() {
    const length = 20;

    const upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const lower = "abcdefghijklmnopqrstuvwxyz";
    const digits = "0123456789";
    const symbols = "!@#$%^&*()-_=+[]{}<>?";

    const all = upper + lower + digits + symbols;

    // Helper: secure random pick
    function pick(str) {
        const array = new Uint32Array(1);
        window.crypto.getRandomValues(array);
        return str[array[0] % str.length];
    }

    let pwd = "";

    // Enforce minimum complexity
    pwd += pick(upper);
    pwd += pick(lower);
    pwd += pick(digits);
    pwd += pick(symbols);

    // Fill the rest
    for (let i = pwd.length; i < length; i++) {
        pwd += pick(all);
    }

    // Shuffle the password to remove predictable order
    pwd = pwd.split("")
        .sort(() => (Math.random() - 0.5))
        .join("");

    document.getElementById("pwdBox").value = pwd;

    // Show regenerate button if you use one
    const regen = document.getElementById("regenBtn");
    if (regen) regen.classList.remove("d-none");
}


function togglePwd() {
    const box = document.getElementById("pwdBox");
    box.type = box.type === "password" ? "text" : "password";
}

function copyPwd() {
    const box = document.getElementById("pwdBox");
    if (box && box.value) {
        navigator.clipboard.writeText(box.value);
    }
}

document.addEventListener("input", function () {
    const box = document.getElementById("pwdBox");
    const meter = document.getElementById("strengthMeter");

    if (!box || !meter) return;

    const pwd = box.value;

    let score = 0;
    if (pwd.length >= 12) score++;
    if (/[A-Z]/.test(pwd)) score++;
    if (/[a-z]/.test(pwd)) score++;
    if (/[0-9]/.test(pwd)) score++;
    if (/[^A-Za-z0-9]/.test(pwd)) score++;

    const levels = ["Very Weak", "Weak", "Medium", "Strong", "Very Strong"];
    meter.innerText = levels[score] || "";
});
