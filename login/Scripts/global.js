function createWndL(wnd, url, titre, w, h) {
    var window1 = $("#" + wnd);
    if (!window1.data("kendoWindow")) {
        $("<div id='" + wnd + "' ></div>").appendTo($("#windowP")).kendoWindow({
            width: w,
            height: h,
            modal: true,
            visible: false,
            title: titre,
            content: URLROOT + url,
            activate: function () { this.center(); },
            close: function (e) {
                this.destroy();
            },
            actions: [
            "Close"
            ]
        });
    };
}