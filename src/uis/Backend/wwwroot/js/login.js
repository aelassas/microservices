import settings from "./settings.js";
import common from "./common.js";

window.onload = () => {
    "use strict";

    window.localStorage.removeItem("auth");

    function login() {
        const user = {
            "email": document.getElementById("email").value,
            "password": document.getElementById("password").value
        };
        common.post(settings.uri + "identity/login?d=backend", function (token) {
            const auth = {
                "email": user.email,
                "token": token
            };
            window.localStorage.setItem("auth", JSON.stringify(auth));
            window.location = "/store.html";
        }, () => {
            alert("Wrong credentials.");
        }, user);
    };

    document.getElementById("login").onclick = () => {
        login();
    };

    document.getElementById("password").onkeyup = (e) => {
        if (e.key === 'Enter') {
            login();
        }
    };
};