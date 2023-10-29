import settings from "./settings.js";
import common from "./common.js";

window.onload = () => {
    "use strict";

    const auth = JSON.parse(window.localStorage.getItem("auth"));

    if (!auth) {
        window.location = "/index.html";
    }

    common.get(settings.uri + "identity/validate?email=" + encodeURIComponent(auth.email) + "&token=" + encodeURIComponent(auth.token), (userId) => {
        common.get(settings.uri + "catalog", (data) => {
            const catalogItems = JSON.parse(data);
            const items = [];
            for (const catalogItem of catalogItems) {
                items.push("<tr>"
                    + "<td class='id'>" + catalogItem.id + "</td>"
                    + "<td class='name'>" + catalogItem.name + "</td>"
                    + "<td class='desc'>" + catalogItem.description + "</td>"
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
            for (const row of rows) {
                const addButton = row.querySelector(".add");
                addButton.onclick = () => {
                    const catalogItemId = row.querySelector(".id").innerHTML;
                    const catalogItemName = row.querySelector(".name").innerHTML;
                    const catalogItemDesc = row.querySelector(".desc").innerHTML;
                    const catalogItemPrice = Number.parseFloat(row.querySelector(".price").innerHTML.replace("$ ", ""));
                    const quantity = 1;

                    const cartItem = {
                        "catalogItem": {
                            "id": catalogItemId,
                            "name": catalogItemName,
                            "Description": catalogItemDesc,
                            "price": catalogItemPrice
                        },
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
    }, (err) => {
        window.location = "/index.html";
    });

};