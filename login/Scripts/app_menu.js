$idGlobalmenu = null;
var MenuH = null;
function onselecttreeMenu(e) {
    var tr = $("#treeviewMenu").data("kendoTreeView");
    var it = tr.select();
    var idm = tr.dataItem(it).id;
    $idGlobalmenu = idm;

    $("#gridMenu").data("kendoGrid").dataSource.read();
}


function getdataMenu() {
    return { idM: $idGlobalmenu };
}

function OnRoleChangeMenu() {
    if ($("#dropRoleMenu").val() != "") {
        $("#treeviewConfigMenu").data("kendoTreeView").dataSource.read({ idR: $("#dropRoleMenu").val() });

    }
}


//----------------Pour la page Config Menu---------------------//

function envoi() {

    var checkedNodes = [];
    var treeView = $("#treeviewConfigMenu").data("kendoTreeView");
    checkedNodeIds(treeView.dataSource.view(), checkedNodes);

    $.ajax({
        url: URLROOT + "Menu/InsertConfMenu",
        type: "POST",
        data: JSON.stringify({ checkedNodes: checkedNodes, role: $("#dropRoleMenu").val() }),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            alert("ok");
        },
        //error: function (e) {
        //    alert("Bien Ajouter")
        //}

    });
}
var localDataSourceC = new kendo.data.HierarchicalDataSource({
    transport: {
        read: {
            url: URLROOT + "Menu/readMenuJ",
            type: "POST",
            dataType: "json"
        }
    },
    schema: {
        model: {
            children: "items",
            hasChildren: function (item) {
                return item.items.length > 0;
            }
        }
    }
});

function checkedNodeIds(nodes, checkedNodes, type) {
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].checked && (typeof type === "undefined" || nodes[i].type == type)) {
            checkedNodes.push(nodes[i].id);
        }

        if (nodes[i].hasChildren) {
            checkedNodeIds(nodes[i].children.view(), checkedNodes, type);
        }
    }
}


function onCheck() {

    var checkedNodes = [],
        treeView = $("#treeviewConfigMenu").data("kendoTreeView"),
          message;

    checkedNodeIds(treeView.dataSource.view(), checkedNodes);

    if (checkedNodes.length > 0) {
        message = "IDs of checked nodes: " + checkedNodes.join(",");
    } else {
        message = "No nodes checked.";
    }

    $("#result").html(message);


}

//------------Pour la page Gerer Menu-----------------//

var localDataSource = new kendo.data.HierarchicalDataSource({
    transport: {
        read: {
            url: URLROOT + "Menu/readMenuJ",
            type: "POST",
            contentType: "application/json",
            dataType: "json"
        }

    }
       ,
    schema: {
        model: {
            children: "items",
            hasChildren: function (item) {
                return item.items.length > 0;
            }
        }
    }
});

//------------------------------//

function getMenu(callback) {
    var datamenu = null;
    $.ajax({
        url: URLROOT + "menu/readMenuP?idR=1",
        type: "POST",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            localStorage.setItem('MenuP', JSON.stringify(data));
            callback(data);
        },
        error: function (e) {
            alert("erreur")
        }
    });
}

//------------------------------//

function initMenu() {
    var old = JSON.parse(localStorage.getItem('MenuP'));
    var newMenu = { enabled: true, text: "", url: "", items: old, cssClass: "IconMenuP" };
    $("#menuP").data("kendoMenu").setOptions({
        dataSource: newMenu,
        // direction: "bottom left"
    });
}

$(document).ready(function () {
    var dataMenuresult = null;
    // console.log(localStorage.getItem('MenuP'));
    if (localStorage.getItem('MenuP')) {
        dataMenuresult = JSON.parse(localStorage.getItem('MenuP'));
        var newMenu = { enabled: true, text: "", url: "", items: dataMenuresult, cssClass: "fa fa-bars" };
        $("#menuP").kendoMenu({
            dataSource: newMenu,
        });

        $("#menuP .k-link.k-menu-link:eq(0)").hide();

        // initLinks();
        MenuH = new kendo.data.HierarchicalDataSource({
            data: newMenu,
            schema: {
                model: {
                    children: "items",
                    hasChildren: function (item) {
                        return item.items.length > 0;
                    }
                }
            }
        });
    } else {
        getMenu(function (e) {
            var newMenu = { enabled: true, text: "", url: "", items: e, cssClass: "fa fa-bars" };
            $("#menuP").kendoMenu({
                dataSource: newMenu,
            });
            // initMenu();
            // initLinks();
            $("#menuP .k-link.k-menu-link:eq(0)").hide();
        });
        dataMenuresult = JSON.parse(localStorage.getItem('MenuP'));
    }
    //titreMenu();
});


//function titreMenu() {
//    console.log("TITRE MENU")
//    var menuArray = JSON.parse(localStorage.getItem('MenuP'));
//    if (menuArray != null) {
//        var menuObj = $.grep(menuArray, function (e) { return e.url == window.location.pathname });
//        if (menuObj.length > 0)
//            $("#titleMenu").text(menuObj[0].title);
//        else
//            $("#titleMenu").text("");

//    }
//}


