import settings from "./settings.js";
import common from "./common.js";

window.onload = () => {
    "use strict";

    const auth = JSON.parse(window.localStorage.getItem("auth"));

    if (!auth) {
        window.location = "/index.html";
    }

    common.get(settings.uri + "identity/validate?email=" + encodeURIComponent(auth.email) + "&token=" + encodeURIComponent(auth.token), function (userId) {
        userId = JSON.parse(userId);
        common.get(settings.uri + "cart?u=" + encodeURIComponent(userId), (cartItems) => {
            cartItems = JSON.parse(cartItems);

            let items = [];
            for (let i = 0; i < cartItems.length; i++) {
                const cartItem = cartItems[i];
                items.push("<tr>"
                    + "<td class='id'>" + cartItem.catalogItemId + "</td>"
                    + "<td class='name'>" + cartItem.catalogItemName + "</td>"
                    + "<td class='price'>" + cartItem.catalogItemPrice + "</td>"
                    + "<td>" + cartItem.quantity + "</td>"
                    + "<td><input type='button' value='Remove' class='remove btn btn-danger' /></td>"
                    + "</tr>");
            }
            let table = "<table class='table'>"
                + "<thead class='table-dark'>"
                + "<tr>"
                + "<th>Name</th>"
                + "<th>Price</th>"
                + "<th>Quantity</th>"
                + "<th></th>"
                + "</tr>"
                + "</thead>"
                + "<tbody>"
                + items.join("")
                + "</tbody>"
                + "</table>";
            document.querySelector(".cart").innerHTML = table;

            const rows = document.querySelector(".cart").getElementsByTagName("tbody")[0].getElementsByTagName("tr");
            for (let i = 0; i < rows.length; i++) {
                const row = rows[i];
                const removeButton = row.querySelector(".remove");
                removeButton.onclick = () => {
                    const catalogItemId = row.querySelector(".id").innerHTML;

                    common.delete(settings.uri + "cart?u=" + encodeURIComponent(userId) + "&ci=" + encodeURIComponent(catalogItemId), () => {
                        row.remove();
                    }, () => {
                        alert("Error while removing item from cart.");
                    }, auth.token);
                };
            }

        }, () => {
            alert("Error while retrieving cart.");
        }, auth.token);

        document.getElementById("logout").onclick = () => {
            window.localStorage.removeItem("auth");
            window.location = "/index.html";
        };
    }, () => {
        window.location = "/index.html";
    });

};