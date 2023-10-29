import settings from "./settings.js";
import common from "./common.js";

window.onload = () => {
    "use strict";

    const auth = JSON.parse(window.localStorage.getItem("auth"));

    if (!auth) {
        window.location = "/index.html";
    }

    common.get(settings.uri + "identity/validate?email=" + encodeURIComponent(auth.email) + "&token=" + encodeURIComponent(auth.token), (userId) => {
        userId = JSON.parse(userId);
        common.get(settings.uri + "catalog", (catalogItems) => {
            catalogItems = JSON.parse(catalogItems);

            const items = [];
            for (let i = 0; i < catalogItems.length; i++) {
                const catalogItem = catalogItems[i];
                items.push("<tr>"
                    + "<td class='id'>" + catalogItem.id + "</td>"
                    + "<td class='name'>" + catalogItem.name + "</td>"
                    + "<td>" + catalogItem.description + "</td>"
                    + "<td class='price'>" + `$ ${catalogItem.price}` + "</td>"
                    + "<td><input type='button' value='Add' class='add btn btn-primary' /></td>"
                    + "</tr>");
            }
            const table = "<table class='table'>"
                + "<thead class='table-dark'>"
                + "<tr>"
                + "<th>Name</th>"
                + "<th>Description</th>"
                + "<th>Price</th>"
                + "<th></th>"
                + "</tr>"
                + "</thead>"
                + "<tbody>"
                + items.join("")
                + "</tbody>"
                + "</table>";
            document.querySelector(".catalog").innerHTML = table;

            const rows = document.querySelector(".catalog").getElementsByTagName("tbody")[0].getElementsByTagName("tr");
            for (let i = 0; i < rows.length; i++) {
                const row = rows[i];
                const addButton = row.querySelector(".add");
                addButton.onclick = () => {
                    const catalogItemId = row.querySelector(".id").innerHTML;
                    const catalogItemName = row.querySelector(".name").innerHTML;
                    const catalogItemPrice = Number.parseFloat(row.querySelector(".price").innerHTML.replace("$ ", ""));
                    const quantity = 1;

                    const cartItem = {
                        "catalogItemId": catalogItemId,
                        "catalogItemName": catalogItemName,
                        "catalogItemPrice": catalogItemPrice,
                        "quantity": quantity
                    };

                    common.post(settings.uri + "cart?u=" + encodeURIComponent(userId), () => {
                        alert("Item added to cart.");
                    }, () => {
                        alert("Error while adding item to cart.");
                    }, cartItem, auth.token);
                };
            }

        }, () => {
            alert("Error while retrieving catalog.");
        }, auth.token);

        document.getElementById("logout").onclick = () => {
            window.localStorage.removeItem("auth");
            window.location = "/index.html";
        };
    }, () => {
        window.location = "/index.html";
    });

};