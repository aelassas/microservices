import settings from "./settings.js";
import common from "./common.js";

window.onload = () => {
    "use strict";

    const auth = JSON.parse(window.localStorage.getItem("auth"));

    if (!auth) {
        window.location = "/index.html";
    }

    common.get(settings.uri + "identity/validate?email=" + encodeURIComponent(auth.email) + "&token=" + encodeURIComponent(auth.token), () => {
        let modal;
        function loadCatalog() {
            common.get(settings.uri + "catalog", (data) => {
                const catalogItems = JSON.parse(data);
                const items = [];
                for (const catalogItem of catalogItems) {
                    items.push("<tr>"
                        + "<td class='id'>" + catalogItem.id + "</td>"
                        + "<td class='name'>" + catalogItem.name + "</td>"
                        + "<td class='desc'>" + catalogItem.description + "</td>"
                        + "<td class='price'>" + `$ ${catalogItem.price}` + "</td>"
                        + "<td>"
                        + "<input type='button' value='Update' class='update btn btn-success' />"
                        + "<input type='button' value='Remove' class='remove btn btn-danger' />"
                        + "</td>"
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
                    const removeButton = row.querySelector(".remove");
                    removeButton.onclick = () => {
                        const catalogItemId = row.querySelector(".id").innerHTML;

                        common.delete(settings.uri + "catalog/" + encodeURIComponent(catalogItemId), () => {

                            // Remove catalog item references from carts
                            common.delete(settings.uri + "cart/delete-catalog-item?ci=" + encodeURIComponent(catalogItemId), () => {
                                row.remove();
                            }, () => {
                                alert("Error while removing item.");
                            }, auth.token);

                        }, () => {
                            alert("Error while removing item.");
                        }, auth.token);
                    };
                    row.querySelector(".update").onclick = () => {
                        if (modal) {
                            modal.destroy();
                        }

                        modal = new jBox("Modal", {
                            width: 800,
                            height: 420,
                            title: "Update Catalog Item",
                            content: document.getElementById("item").innerHTML,
                            footer: document.getElementById("item-footer").innerHTML,
                            overlay: true,
                            delayOpen: 0,
                            onOpen: () => {
                                const jBoxContent = document.getElementsByClassName("jBox-content")[0];
                                const jBoxFooter = document.getElementsByClassName("jBox-footer")[0];
                                const submitButton = jBoxFooter.querySelector(".submit");

                                jBoxContent.querySelector(".name").value = row.querySelector(".name").innerHTML;
                                jBoxContent.querySelector(".desc").value = row.querySelector(".desc").innerHTML;
                                jBoxContent.querySelector(".price").value = row.querySelector(".price").innerHTML.replace("$ ", "");

                                submitButton.onclick = () => {
                                    const id = row.querySelector(".id").innerHTML;
                                    const name = jBoxContent.querySelector(".name").value;
                                    const description = jBoxContent.querySelector(".desc").value;
                                    const price = Number.parseFloat(jBoxContent.querySelector(".price").value);

                                    if (name !== "" && description !== "") {
                                        const catalogItem = {
                                            id,
                                            name,
                                            description,
                                            price
                                        };

                                        common.put(settings.uri + "catalog", () => {

                                            common.put(settings.uri + `cart/update-catalog-item?ci=${id}&n=${encodeURIComponent(name)}&p=${encodeURIComponent(price)}`, () => {
                                                modal.destroy();
                                                loadCatalog();
                                            }, () => {
                                                alert("Error while updating catalog item.");
                                            }, catalogItem, auth.token);

                                        }, () => {
                                            alert("Error while updating catalog item.");
                                        }, catalogItem, auth.token);
                                    } else {
                                        alert("Error: Empty values.");
                                    }
                                };
                            },
                            onClose: () => {
                            }
                        });

                        modal.open();
                    };
                }

            }, () => {
                alert("Error while retrieving catalog.");
            }, auth.token);
        }

        loadCatalog();

        document.getElementById("add").onclick = () => {
            if (modal) {
                modal.destroy();
            }

            modal = new jBox("Modal", {
                width: 800,
                height: 420,
                title: "New Catalog Item",
                content: document.getElementById("item").innerHTML,
                footer: document.getElementById("item-footer").innerHTML,
                overlay: true,
                delayOpen: 0,
                onOpen: () => {
                    const jBoxContent = document.getElementsByClassName("jBox-content")[0];
                    const jBoxFooter = document.getElementsByClassName("jBox-footer")[0];
                    const submitButton = jBoxFooter.querySelector(".submit");
                    submitButton.onclick = () => {
                        const name = jBoxContent.querySelector(".name").value;
                        const description = jBoxContent.querySelector(".desc").value;
                        const price = Number.parseFloat(jBoxContent.querySelector(".price").value);

                        if (name !== "" && description !== "") {
                            const catalogItem = {
                                name,
                                description,
                                price
                            };

                            common.post(settings.uri + "catalog", () => {
                                modal.destroy();
                                loadCatalog();
                            }, () => {
                                alert("Error while creating catalog item.");
                            }, catalogItem, auth.token);
                        } else {
                            alert("Error: Empty values.");
                        }
                    };
                },
                onClose: () => {
                }
            });

            modal.open();
        };

        document.getElementById("logout").onclick = () => {
            window.localStorage.removeItem("auth");
            window.location = "/index.html";
        };
    }, () => {
        window.location = "/index.html";
    });

};