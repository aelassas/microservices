export default {
    post: async (url, callback, errorCallback, content, token) => {
        try {
            const headers = {
                "Content-Type": "application/json;charset=UTF-8"
            };
            if (token) {
                headers["Authorization"] = `Bearer ${token}`;
            }
            const response = await fetch(url, {
                method: "POST",
                headers,
                body: JSON.stringify(content)
            });
            if (response.ok) {
                const data = await response.text();
                if (callback) {
                    callback(data);
                }
            } else {
                if (errorCallback) {
                    errorCallback(response.status);
                }
            }
        } catch (err) {
            if (errorCallback) {
                errorCallback(err);
            }
        }
    },
    put: async (url, callback, errorCallback, content, token) => {
        try {
            const headers = {
                "Content-Type": "application/json;charset=UTF-8"
            };
            if (token) {
                headers["Authorization"] = `Bearer ${token}`;
            }
            const response = await fetch(url, {
                method: "PUT",
                headers,
                body: JSON.stringify(content)
            });
            if (response.ok) {
                const data = await response.text();
                if (callback) {
                    callback(data);
                }
            } else {
                if (errorCallback) {
                    errorCallback(response.status);
                }
            }
        } catch (err) {
            if (errorCallback) {
                errorCallback(err);
            }
        }
    },
    get: async (url, callback, errorCallback, token) => {
        try {
            const headers = {
                "Content-Type": "application/json;charset=UTF-8"
            };
            if (token) {
                headers["Authorization"] = `Bearer ${token}`;
            }
            const response = await fetch(url, {
                method: "GET",
                headers
            });
            if (response.ok) {
                const data = await response.text();
                if (callback) {
                    callback(data);
                }
            } else {
                if (errorCallback) {
                    errorCallback(response.status);
                }
            }
        } catch (err) {
            if (errorCallback) {
                errorCallback(err);
            }
        }
    },
    delete: async (url, callback, errorCallback, token) => {
        try {
            const headers = {
                "Content-Type": "application/json;charset=UTF-8"
            };
            if (token) {
                headers["Authorization"] = `Bearer ${token}`;
            }
            const response = await fetch(url, {
                method: "DELETE",
                headers
            });

            if (response.ok) {
                if (callback) {
                    callback();
                }
            } else {
                if (errorCallback) {
                    errorCallback(response.status);
                }
            }
        } catch (err) {
            if (errorCallback) {
                errorCallback(err);
            }
        }
    }
};