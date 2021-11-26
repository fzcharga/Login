//var DsUser = new kendo.data.DataSource({
//    type: "aspnetmvc-ajax",
//    transport: {
//        read: {
//            async: false,
//            url: URLROOT + "FormationAdmin/ReadUser",
//            type: "POST"
//        }
//    },
//    schema: {
//        model: {
//            id: "id",
//        }
//    }
//});


/* Fin Abonnes */

var DsAbonnes = new kendo.data.DataSource({
    transport: {
        read: {
            url: URLROOT + "Abonne/ReadAbonne",
            type: "POST",
        }
    }
});

var selectedType = null;

var isAllType = false;

var DsType = new kendo.data.DataSource({
    type: "aspnetmvc-ajax",
    batch: true,
    transport: {
        read: {
            async: false,
            url: URLROOT + "TypeDepense/ReadType",
            type: "GET",
            data: function () {
                return {
                    type: selectedType,
                    isAllType: isAllType
                }
            }
        },
        update: {
            // async: false,
            url: URLROOT + "TypeDepense/UpdateType",
            type: "POST",

        },
        create: {
            // async: false,
            url: URLROOT + "TypeDepense/CreateType",
            type: "POST",
            data: function () {
                return {
                    type: selectedType
                }
            }
        },
        destroy: {
            url: URLROOT + "TypeDepense/DeleteType",
            type: "POST",
        }
    },
    schema: {
        model: {
            id: "id",
            fields: {
                id: { field: "id", type: "number" },
                lb: { field: "lb", type: "string" }

            }
        }
    },
    requestEnd: function (e) {
        if (e.type !== "read") {
            var message = "";
            for (var i = 0 ; i < e.response.length ; i++) {
                var m = e.response[i].message;
                if (m != null)
                    message += m + "\n";
            }
            if (message != "") {
                notification.show({ title: "", message: message }, "error");
                this.read();
            }
        }
    }
});

var etatDS = new kendo.data.DataSource({
    transport: {
        read: {
            async: false,
            url: URLROOT + "Orga/ReadEtatData",
            type: "POST",
            contentType: "application/json",
            dataType: "json"
        }
    },
    schema: {
        model: {
            id: "id"
        }
    }
});
//_________________________________________ DsPieces_J :

var DsPieces_J = new kendo.data.DataSource({
    transport: {
        read: {
            type: "POST",
            url: URLROOT + "Depense/GetPiecesJ",
        }
    },
    schema: {
        model: {
            id: "fileName",
            fields: {
                fileName: { type: "string" },
            }
        }
    }
});
//_________________________________________ Dslogin :

var Dslogin = new kendo.data.DataSource({
    type: "aspnetmvc-ajax",
    transport: {
        read: {
            url: URLROOT + "login/Readlogin",
            type: "GET",
        },
        update: {
            url: URLROOT + "login/UpdateDepense",
            type: "POST",
        },
        destroy: {
            url: URLROOT + "login/DestroyDepense",
            type: "POST",
        },

        create: {
            url: URLROOT + "login/CreateDepense",
            type: "POST",
        },
    },
    schema: {
        model: {
            fields: {
                idDepense: { type: "number" },
                idDonneur: { type: "string" },
                credit: { type: "number" },
                idPrenneur: { type: "string" },
                debit: { type: "number" },
                date: { type: "date", },
                solde: { type: "number" },
                typeDepense: { type: "number" },
                description/*Dp*/: { type: "string" },
                hasFiles: { type: "boolean" }
                //descriptionAlm: {type: "string"}
            }
        }
    },
    group: { field: "idUser" },
    //requestEnd: function (e) {
    //    //localStorage.setItem("solde", e.response[0].solde);
    //    if (e.response.length != 0)
    //        sessionStorage.setItem("solde", e.response[0].solde);
    //    else
    //        sessionStorage.setItem("solde", "0.00");
    //}
});
//________________________________________ refresheSolde :

function refresheSolde() {
    $.ajax({
        type: "POST",
        //url: URLROOT + "login/getSolde",
        url: URLROOT + "login/readlogin?group=true&solde=true",
        data: { idAbonne: OnSelectAbonne },
        success: function (Res) {
            $("#lbSolde").text(Res);

        }
    });
}
//_________________________________________ DsDepense :

var DsDepense = new kendo.data.DataSource({
    type: "aspnetmvc-ajax",
    batch: true,
    transport: {
        read: {
            url: URLROOT + "Depense/ReadDepense",
            type: "GET",
        },
        update: {
            url: URLROOT + "Depense/UpdateDepense",
            type: "POST",
        },
        destroy: {
            url: URLROOT + "Depense/DestroyDepense",
            type: "POST",
        },
        create: {
            url: URLROOT + "Depense/CreateDepense",
            type: "POST",
            data: function () {
                return {
                    idUser: drpUser.value(),
                }
            }
        }
    },
    schema: {
        model: {
            id: "id",
            fields: {
                id: { type: "number", editable: false },
                idUser: { type: "string", editable: false },
                idType: { type: "number" },
                quantite: { type: "number" },
                description: { type: "string", editable: false },
                montant: { type: "number" },
                date: { type: "date", editable: false },
                hasFiles: { type: "boolean" }
            }
        }
    },
    group: {
        field: "idUser",
        aggregates: [
                { field: "montant", aggregate: "sum" },
        ],
    }
});
// 

//var DsDepense = new kendo.data.DataSource({
//    type: "aspnetmvc-ajax",
//    batch: true,
//    transport: {
//        read: {
//            url: URLROOT + "Depense/ReadDepense",
//            type: "GET",
//        },
//        update: {
//            url: URLROOT + "Depense/UpdateDepense",
//            type: "POST",
//        },
//        destroy: {
//            url: URLROOT + "Depense/DestroyDepense",
//            type: "POST",
//        },
//        create: {
//            url: URLROOT + "Depense/CreateDepense",
//            type: "POST",
//        },
//    },
//    requestStart: function () {
//    },
//    requestEnd: function () {
//    },
//    schema: {
//        model: {
//            id: "id",
//            fields: {
//                id: { type: "number" },
//                idUser: { type: "string" },
//                idType: { type: "number" },
//                description: { type: "string" },
//                montant: { type: "number", },
//                date: { type: "date" },
//            }
//        }
//    },
//    pageSize: 10,
//    group: {
//        field: "idType",
//        aggregates: [
//            { field: "montant", aggregate: "sum" },
//        ],
//    },
//    aggregate: [
//         { field: "idUser", aggregate: "count" },
//           { field: "idType", aggregate: "count" },
//        { field: "montant", aggregate: "sum" },
//    ],
//});
//_________________________________________ DsUtilisateur :

var DsUtilisateur = new kendo.data.DataSource({
    type: "aspnetmvc-ajax",
    transport: {
        read: {
            async: false,
            url: URLROOT + "User/ReadUser",
            type: "GET",
            data: OnSelectAbonne
        },
        create: {
            url: URLROOT + "User/CreateUser",
            type: "POST",
            data: OnSelectAbonne
        },
        update: {
            url: URLROOT + "User/UpdateUser",
            type: "POST"
        }
    },
    schema: {
        model: {
            id: "id",
            fields: {
                id: { type: "string" },
                name: { type: "string" },
                civilite: { type: "number", defaultValue: 1 },
                email: { type: "string" },
                password: { type: "string" },
                tel: { type: "string" },
            }
        }
    }
});

//DsUtilisateur.read();

//_________________________________ DsCivilite :

var DsCivilite = new kendo.data.DataSource({
    type: "aspnetmvc-ajax",
    transport: {
        read: {
            async: false,
            url: URLROOT + "User/ReadCivilite",
            type: "GET",
        }
    },
    schema: {
        model: {
            id: "id",
            fields: {
                id: { type: "number" },
                lb: { type: "string" },
            }
        }
    }
});

DsCivilite.read();

var DsUsersBySup = new kendo.data.DataSource({
    type: "aspnetmvc-ajax",
    transport: {
        read: {
            async: false,
            url: URLROOT + "User/ReadUsersBySup",
            type: "GET",
            data: OnSelectAbonne
        }
    },
    schema: {
        model: {
            id: "id",
            fields: {
                id: { type: "string" },
                name: { type: "string" },
                email: { type: "string" },
                password: { type: "string" },
                tel: { type: "string" },
                formations: { editable: false },
                projets: { editable: false }
            }
        }
    }
});

//DsUsersBySup.read();

var DsUsersParent = new kendo.data.DataSource({
    type: "aspnetmvc-ajax",
    transport: {
        read: {
            async: false,
            url: URLROOT + "User/ReadUsersParents",
            type: "GET",
            data: OnSelectAbonne
        }
    },
    schema: {
        model: {
            id: "id",
            fields: {
                id: { type: "string" },
                name: { type: "string" },
                email: { type: "string" },
                password: { type: "string" },
                tel: { type: "string" },
                formations: { editable: false },
                projets: { editable: false }
            }
        }
    }
});

var selectedAbonne = null;

var drpAbonne = null;

function OnSelectAbonne() {
    return {
        idAbonne: selectedAbonne
    }
}

$(function () {
    drpAbonne = $("#drpAbonne").kendoDropDownList({
        dataSource: DsAbonnes,
        dataTextField: "lb",
        dataValueField: "id",
        change: function () {
            selectedAbonne = this.value();
        }
    }).data("kendoDropDownList");

    if (typeof ChangeAbonne !== "undefined") {
        drpAbonne.bind("change", ChangeAbonne);
    }
});

/* Fin Abonnes */

function openMail(idMail) {
    var dialog = $("<div id='wndVisualiserMailExt'><iframe class='k-content-frame' style='height: 842px;'></iframe></div>").kendoWindow({
        width: "845px",
        title: "Visualiser Mail",
        close: function () {
            this.destroy();
        },
        content: URLROOTCRM + "CategorieMail/VisualiserMailExt?idMail=" + idMail,
        iframe: true,
        modal: true,
        statut: null,
    }).data("kendoWindow");

    dialog.center().open();
}

function showLoad(selector) {
    if (typeof selector !== "undefined")
        kendo.ui.progress($(selector), true);
    else
        kendo.ui.progress($("body"), true);
}

function hideLoad(selector) {
    if (typeof selector !== "undefined")
        kendo.ui.progress($(selector), false);
    else
        kendo.ui.progress($("body"), false);
}

var DsApplications = new kendo.data.DataSource({
    type: "aspnetmvc-ajax",
    batch: true,
    transport: {
        read: {
            async: false,
            url: URLROOT + "Applications/ReadApplications",
            type: "POST",
        },
        create: {
            url: URLROOT + "Applications/CreateApplications",
            type: "POST",
        },
        update: {
            url: URLROOT + "Applications/UpdateApplications",
            type: "POST",
        },
        destroy: {
            url: URLROOT + "Applications/DestroyApplications",
            type: "POST",
        }
    },
    schema: {
        model: {
            id: "id",
            fields: {
                id: { type: "number"/*, editable: false*/ },
                name: { type: "string" },
                desc: { type: "string" },
                url: { type: "string" },
                notUseLogin: { type: "boolean" },
            }
        }
    }
});

var DsApplicationsUsers = new kendo.data.DataSource({
    type: "aspnetmvc-ajax",
    batch: true,
    transport: {
        read: {
            async: false,
            url: URLROOT + "Applications/ReadApplicationsUsers",
            type: "POST",
        },
        create: {
            url: URLROOT + "Applications/CreateApplicationsUsers",
            type: "POST",
        },
        update: {
            url: URLROOT + "Applications/UpdateApplicationsUsers",
            type: "POST",
        },
        destroy: {
            url: URLROOT + "Applications/DestroyApplicationsUsers",
            type: "POST",
        }
    },
    schema: {
        model: {
            id: "id",
            fields: {
                id: { type: "number" },
                idApp: { type: "number" },
            }
        }
    }
});