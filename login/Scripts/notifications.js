var notification = $("#notification").kendoNotification({
    position: {
        pinned: true,
        bottom: 30,
        right: 30
    },
    autoHideAfter: 3000,
    stacking: "up",
    templates: [{
        type: "info",
        template: $("#emailTemplate").html()
    }, {
        type: "error",
        template: $("#errorTemplate").html()
    }, {
        type: "success",
        template: $("#successTemplate").html()
    }]
}).data("kendoNotification");

var notificationNoHide = $("#notificationNoHide").kendoNotification({
    position: {
        pinned: true,
        bottom: 30,
        right: 30
    },
    autoHideAfter: 0,
    stacking: "up",
    button: true,
    templates: [{
        type: "info",
        template: $("#emailTemplate").html()
    }, {
        type: "error",
        template: $("#errorTemplate").html()
    }, {
        type: "success",
        template: $("#successTemplate").html()
    }]
}).data("kendoNotification");

$(document).one("kendo:pageUnload", function () { if (notification) { notification.hide(); } });


//notification.show({
//    title: "New E-mail",
//    message: "You have 1 new mail message!"
//}, "success");


//notification.show({
//    title: "New E-mail",
//    message: "You have 1 new mail message!"
//}, "info");


//notification.show({
//    title: "New E-mail",
//    message: "You have 1 new mail message!You have 1 new mail message!You have 1 new mail message!You have 1 new mail message!"
//}, "error");