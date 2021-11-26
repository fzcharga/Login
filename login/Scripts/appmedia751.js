var DsExtensions = new kendo.data.DataSource({
    transport: {
        read: {
            url: URLROOT + "Extensions/Read",
            type: "POST",
        }
    }
});

DsExtensions.read();

function urlFichierByCode(file, dossier, controller) {
    var res = "";
    if (file != null) {
        switch (file.typeFichier) {
            case "pdf": res = "<a href='javascript:void(0)' onclick='affPDF(\"" + URLROOT + dossier + "/" + file.code + file.ext + "\",\"" + file.lb + "\")'><div class='extension' style='background-image:url(" + URLROOT + "ExtensionsFichiers/" + file.ext.substring(1) + ".png);'></div>" + file.lb + "</a>"; break;
            case "image": res = "<a href='javascript:void(0)' onclick='affImage(\"" + URLROOT + dossier + "/" + file.code + file.ext + "\",\"" + file.lb + "\")'><div class='extension' style='background-image:url(" + URLROOT + "ExtensionsFichiers/" + file.ext.substring(1) + ".png);'></div>" + file.lb + "</a>"; break;
            case "audio": res = "<a href='javascript:void(0)' onclick='affAudio(\"" + URLROOT + dossier + "/" + file.code + file.ext + "\",\"" + file.lb + "\")'><div class='extension' style='background-image:url(" + URLROOT + "ExtensionsFichiers/" + file.ext.substring(1) + ".png);'></div>" + file.lb + "</a>"; break;
            case "video": res = "<a href='javascript:void(0)' onclick='affVideo(\"" + URLROOT + dossier + "/" + file.code + file.ext + "\",\"" + file.lb + "\")'><div class='extension' style='background-image:url(" + URLROOT + "ExtensionsFichiers/" + file.ext.substring(1) + ".png);'></div>" + file.lb + "</a>"; break;
            default: res = "<a href='" + URLROOT + controller + "/download?idAttach=" + file.id + "'><div class='extension' style='background-image:url(" + URLROOT + "ExtensionsFichiers/" + file.ext.substring(1) + ".png);'></div>" + file.lb + "</a>";
        }
    }
    return res;
}

function urlFichier(id, fileName, dossier, controller) {
    var res = "";
    var ext = "." + fileName.split('.').pop();
    var extDB = DsExtensions.get(ext.toString().toLowerCase());
    var type = "";
    if(extDB != null)
        type = extDB.type;
    switch (type) {
        case "pdf": res = "<a href='javascript:void(0)' onclick='affPDF(\"" + URLROOT + dossier + "/" + id + "/" + fileName + "\",\"" + fileName + "\")'>" + fileName + "</a>"; break;
        case "image": res = "<a href='javascript:void(0)' onclick='affImage(\"" + URLROOT + dossier + "/" + id + "/" + fileName + "\",\"" + fileName + "\")'>" + fileName + "</a>"; break;
        case "audio": res = "<a href='javascript:void(0)' onclick='affAudio(\"" + URLROOT + dossier + "/" + id + "/" + fileName + "\",\"" + fileName + "\")'>" + fileName + "</a>"; break;
        case "video": res = "<a href='javascript:void(0)' onclick='affVideo(\"" + URLROOT + dossier + "/" + id + "/" + fileName + "\")'>" + fileName + "</a>"; break;
        default: res = "<a href='" + URLROOT + controller + "/download?id=" + id + "&fileName=" + fileName + "'>" + fileName + "</a>";
    }

    return res;
}

function affPDF(url, titre) {
    createWndF751("WindowsPDF", "<object data='" + url + "' type='application/pdf' title='cv' style='height:100%;width:100%;min-height:800px;'></object>", titre, "80%");
    $("#WindowsPDF").data("kendoWindow").open().center();
}

function affImage(url, titre) {
    createWndF751("WindowsImage", "<img src='" + url + "' class='img-responsive' style='margin-left: auto;margin-right: auto;' />", titre, "80%");
    $("#WindowsImage").data("kendoWindow").open().center();
}

function affVideo(url) {
    var tag2 = "<video width='100%'  controls>" +
                    "<source src='" + url + "' type='video/ogg'/>" +
                    "<source src='" + url + "'  type='video/mp4'/>" +
                "</video>";
    createWndF751Video("WindowsVideo", tag2, "Lecture de la video", "80%");
    $("#WindowsVideo").data("kendoWindow").open().center();
}

function affVideo2() {
    var tag = "<video controls><source src='mavidéo.ogg' type='video/ogg'/><source src='mavidéo.mp4' type='video/mp4'/>" +
                "<object src='http://blip.tv/play/AYGLzBmU8hw'" +
                "type='application/x-shockwave-flash'" +
                "width='500' height='396'" +
                "allowscriptaccess='always'" +
                "allowfullscreen='true'/>" +
                "</video>";

    createWndF751Video("WindowsVideo", tag, titre, "80%");
    $("#WindowsVideo").data("kendoWindow").open().center();
}

function affAudio(url, titre) {
    var tag = "<audio id='audioPlayer' controls='controls'style='width: 100%'><source src='" + url + "'></audio>";
    createWndF751("WindowsAudio", tag, titre, "80%");
    $("#WindowsAudio").data("kendoWindow").open().center();
}

function createWndF751(wnd, element, titre, taille) {
    var window1 = $("#" + wnd);
    if (!window1.data("kendoWindow")) {
        $("<div id='" + wnd + "' >" + element + "</div>").appendTo($("#windowFichier")).kendoWindow({
            width: taille,
            visible: false,
            title: titre,
            modal: true,
            close: function (e) {
                this.destroy();
            },
            actions: [
                "Close"
            ]
        });
    }
}

function createWndF751Video(wnd, element, titre, taille) {
    var window1 = $("#" + wnd);
    if (!window1.data("kendoWindow")) {
        $("<div id='" + wnd + "' >" + element + "</div>").appendTo($("#windowFichier")).kendoWindow({
            width: taille,
            visible: false,
            resizable: false,
            title: titre,
            modal: true,
            close: function (e) {
                this.destroy();
            },
            actions: [
                "Close"
            ]
        });
    }
}