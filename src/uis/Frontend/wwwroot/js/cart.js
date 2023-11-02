import settings from "./settings.js";
import common from "./common.js";

window.onload = () => {
    "use strict";

    const auth = JSON.parse(localStorage.getItem("auth"));

    if (!auth) {
        location.href = "/index.html";
    }

    common.get(settings.uri + "identity/validate?email=" + encodeURIComponent(auth.email) + "&token=" + encodeURIComponent(auth.token), function (userId) {
        common.get(settings.uri + "cart?u=" + encodeURIComponent(userId), (data) => {
            const cartItems = JSON.parse(data);

            let items = [];
            for (const cartItem of cartItems) {
                items.push("<tr>"
                    + "<td class='id'>" + cartItem.catalogItemId + "</td>"
                    + "<td class='name'>" + cartItem.name + "</td>"
                    + "<td class='price'>" + `$ ${cartItem.price}` + "</td>"
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
            for (const row of rows) {
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
            localStorage.removeItem("auth");
            location.href = "/index.html";
        };
    }, () => {
        location.href = "/index.html";
    });

};