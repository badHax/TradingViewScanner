self.addEventListener(
    "notificationclick",
    (event) => {
        event.notification.close();
        if (event.action === "openLinkInNewTab") {
            const url = event.data;
            if (url) {
                console.log("openLinkInNewTab", url);
                event.waitUntil(clients.openWindow(url));
            }
        }
    },
    false,
);