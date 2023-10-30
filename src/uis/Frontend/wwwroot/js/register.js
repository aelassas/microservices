import settings from "./settings.js";
import common from "./common.js";

window.onload = () => {
    "use strict";

    document.getElementById("register").onclick = () => {
        const email = document.getElementById("email").value;
        const password = document.getElementById("password").value;
        const confirmedPassword = document.getElementById("confirm-password").value;

        if (password !== "" && password === confirmedPassword) {
            const user = {
                "email": email,
                "password": password
            };

            common.post(settings.uri + "identity/register", () => {
                common.post(settings.uri + "identity/login?d=frontend", (token) => {
                    const auth = {
                        "email": user.email,
                        "token": token
                    };
                    localStorage.setItem("auth", JSON.stringify(auth));
                    location.href = "/store.html";
                }, () => {
                    alert("Login error.");
                }, user);
            }, () => {
                alert("Registration error.");
            }, user);
        } else {
            alert("Passwords don't match.");
        }
    };
};