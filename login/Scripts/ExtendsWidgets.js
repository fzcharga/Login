
function HightLightSearchWidget(containerId, value) {
    var v = value.toString().toLowerCase().trim();
    if (v == "") {
        $('.k-group#' + containerId + ' > li').show();
    }
    else {
        $('.k-group#' + containerId + ' > li').hide();
        $('.k-group#' + containerId + ' > li').each(function () {
            var clone = $(this).clone();
            clone.find(".my-badge").remove();
            if (clone.text().toLowerCase().indexOf(v) != -1)
                $(this).show();
        });
    }
    var div = document.getElementById(containerId);
    var spans = document.getElementsByClassName('found');
    while (spans.length) {
        var p = spans[0].parentNode;
        p.innerHTML = p.textContent || p.innerText;
    }
    if (value) changeNodeWidget(
      div, new RegExp('(' + value + ')', 'gi')
    );
}

function changeNodeWidget(n, r, f) {
    f = n.childNodes; for (c in f) changeNodeWidget(f[c], r);
    if (n.data) {
        f = document.createElement('span');
        f.innerHTML = n.data.replace(r, '<span class="found">$1</span>');
        n.parentNode.insertBefore(f, n);
        n.parentNode.removeChild(n);
    }
}

(function ($, undefined) {
    var kendo = window.kendo,
        ui = kendo.ui;

    var OmfPanelBar = ui.PanelBar.extend({
        getLastFilter: function (field) {
            var that = this;
            var f = [];
            if (typeof that.options.dataSourceWidget.filter() !== "undefined") {
                var filters = that.options.dataSourceWidget.filter().filters;
                for (var i = 0 ; i < filters.length ; i++) {
                    if (filters[i].fld != field) {
                        f.push(filters[i]);
                    }
                    else
                        return f;
                }
            }
            return f;
        },
        findItemDataSource: function (name) {
            var that = this;
            return $.grep(that.options.dataSource, function (n, i) {
                return n.defaultText == name;
            });
        },
        getTypeField: function (field) {
            var that = this;
            var type = null;
            var ds = that.options.dataSourceWidget;
            var model = ds.options.schema.model;
            if (typeof model !== "undefined") {
                var fields = model.fields;
                if (typeof fields !== "undefined") {
                    var field = fields[field];
                    if (typeof field !== "undefined") {
                        var t = field.type;
                        if (typeof t !== "undefined")
                            type = t;
                    }
                }
            }
            return type;
        },
        filtreElements: function (field, value, event) {
            // console.log(event);
            var that = this;
            var filters = [];
            if (typeof that.options.dataSourceWidget.filter() !== "undefined")
                filters = that.options.dataSourceWidget.filter().filters;
            var indexFieldFilter = $.map(filters, function (ele) { return ele.fld; }).indexOf(field);
            var op = "eq";
            if (event.ctrlKey)
                op = "neq";
            if (indexFieldFilter == -1) {
                // filters.push({ field: field, operator: op, value: value });
                filters.push({
                    fld: field, op: op, operator: function (data) {
                        var v = value;
                        if (v != null)
                            v = v.toString().toUpperCase();
                        var bs = byString(data, field);
                        if (bs != null)
                            bs = bs.toString().toUpperCase();
                        return op == "eq" ? v == bs : v != bs;
                    }, value: value
                });
            }
            else {
                filters[indexFieldFilter] = {
                    fld: field, op: op, operator: function (data) {
                        var v = value;
                        if (v != null)
                            v = v.toString().toUpperCase();
                        var bs = byString(data, field);
                        if (bs != null)
                            bs = bs.toString().toUpperCase();
                        return op == "eq" ? v == bs : v != bs;
                    }, value: value
                };
                for (var i = indexFieldFilter + 1; i < filters.length ; i++) {
                    if (that.selectedFields.indexOf(filters[i].fld) != -1)
                        filters.splice(i--, 1);
                }
            }

            var indexField = that.selectedFields.indexOf(field);
            if (indexField == -1) {
                that.selectedFields.push(field);
                that.selectedItems.push(field + "_" + value);
            }
            else {
                that.selectedItems[indexField] = field + "_" + value;
                that.selectedFields.splice(indexField + 1, that.selectedFields.length);
                that.selectedItems.splice(indexField + 1, that.selectedItems.length);
            }

            that.options.dataSourceWidget.filter({ logic: "and", filters: filters });
            if (that.lvGrouped != null)
                that.setDataSourceLvGrouped();
        },
        init: function (element, options) {
            var that = this;

            if (options != null) {

                for (var i = 0 ; i < options.dataSource.length ; i++) {
                    var d = options.dataSource[i];

                    var defaultOptions = {
                        encoded: false,
                        defaultText: d.text || d.field,
                        text: d.text || d.field,
                        expanded: true,
                        field: d.field,
                        sumField: null,
                        data: {},
                        dsGetLb: null,
                        isSort: false,
                    };
                    d = $.extend({}, defaultOptions, d || {});
                    options.dataSource[i] = d;
                }

                expand = {
                    expand: function (e) {
                        var parent = $(e.item).find(".k-header .m-title-panelbar-item").text();
                        that.findItemDataSource(parent)[0].expanded = true;
                    },
                    collapse: function (e) {
                        var parent = $(e.item).find(".k-header .m-title-panelbar-item").text();
                        that.findItemDataSource(parent)[0].expanded = false;
                    },
                };

                options = $.extend({}, expand, options || {});

                ui.PanelBar.fn.init.call(that, element, $.extend(options, expand));

                if (typeof options.targetSmartMenuId === "undefined")
                    options.targetSmartMenuId = null;

                if (options.targetSmartMenuId != null) {
                    var id = $(that.element[0]).attr("id").trim();
                    var click = "$(\"#" + id + "\").data(\"kendoOmfPanelBar\").removeFilteredItem(this)";
                    that.lvGrouped = $(options.targetSmartMenuId).kendoListView({
                        dataBound: function () {
                            $(".grouped-item").click(function () {
                                var uid = $(this).data("uid");
                                $("#" + id + "").data("kendoOmfPanelBar").removeFilteredItem(uid);
                            });
                        },
                        template: "# var cl = (operator == null ? (neq ? \"grouped-item-neq-selected-field\" : \"grouped-item-selected-field\") : \"\")#" +
                                    "# var cl2 = (neq ? \"grouped-neq-item\" : \"\") #" +
                                    "<span class=\"grouped-item #:cl2# #:cl# \">#:lb# #:operator != null ? \"\" + operator + \"\" : \"\"# #:text#</span>"
                    }).data("kendoListView");
                    if (!$(options.targetSmartMenuId).hasClass("target-smart-menu"))
                        $(options.targetSmartMenuId).addClass("target-smart-menu")
                }
            }

            var id = $(that.element[0]).attr("id").trim();

            for (var i = 0 ; i < that.options.dataSource.length ; i++) {
                that.options.dataSource[i].data = {};
            }

            var vm = that;

            var DsTemp = new kendo.data.DataSource({
                data: that.options.dataSourceWidget.data()
            });

            for (var i = 0 ; i < that.options.dataSource.length ; i++) {
                var dataItem = that.options.dataSource[i];
                var f = that.getLastFilter(dataItem.field);
                DsTemp.filter({ logic: "and", filters: f });

                var count = 0;
                $.each(DsTemp.view(), function (i, value) {
                    var v = byString(value, dataItem.field);
                    var type = that.getTypeField(dataItem.field);
                    var sum = 0;
                    if (dataItem.sumField != null) {
                        var vSum = byString(value, dataItem.sumField);
                        sum = vSum;
                    }
                    if (type == "string" && v != null && typeof v.toUpperCase === "function")
                        v = v.toUpperCase();
                    else {
                        var isnum = /^\d+$/.test(v);
                        if (v != null && dataItem.dsGetLb == null && !isnum && typeof v === "string")
                            v = v.toUpperCase();
                    }
                    if (dataItem.data[v] != null) {
                        dataItem.data[v].count++;
                        dataItem.data[v].sum += sum;
                    } else {
                        dataItem.data[v] = { count: 1, sum: sum };
                        count++;
                    }
                });

                var inputFilter = "<input class='m-input-filter-items k-textbox' />";

                dataItem.text = "<span class='m-title-panelbar-item'>" + dataItem.defaultText + "</span>" + inputFilter + "<span class='content-my-badge'>" + "<span class='badge badge-item-panel'>" + count + "</span>" + "</span>";

                if (dataItem.isSort) {
                    var obj = {};
                    var isDs = dataItem.dsGetLb != null;
                    Object.keys(dataItem.data).sort(function (a, b) {
                        if (!isDs) {
                            if (typeof a === "string")
                                return a.toLowerCase() > b.toLowerCase();
                            return a > b;
                        }
                        else {
                            // console.log(dataItem.dsGetLb.get(a).lb, dataItem.dsGetLb.get(b).lb, dataItem.dsGetLb.get(a).lb > dataItem.dsGetLb.get(b).lb);
                            var lbA = dataItem.dsGetLb.get(a).lb.toLowerCase();
                            var lbB = dataItem.dsGetLb.get(b).lb.toLowerCase()
                            return lbA > lbB;
                        }
                    }).forEach(function (v, i) {
                        obj[v] = dataItem.data[v];
                    });
                    dataItem.data = obj;
                }

                var items = $.map(dataItem.data, function (value, index) {
                    var operator = that.getOperatorByField(dataItem.field);
                    var cssClass = vm.selectedItems.indexOf(dataItem.field + "_" + index) != -1 ? (operator == "eq" ? "k-selected-item" : "k-neq-selected-item") : "";
                    var borderColor = "";
                    if (index == "true" || index == "false")
                        index = (index == "true");
                    if (dataItem.dsGetLb != null && dataItem.dsGetLb.get(index) != null && typeof dataItem.dsGetLb.get(index).color !== "undefined") {
                        borderColor = "style='border-left: 3px solid " + dataItem.dsGetLb.get(index).color + "; padding-left:3px;'";
                    }
                    var coteIndex = "";
                    var type = that.getTypeField(dataItem.field);
                    if (type == "string" && index != "null") {
                        coteIndex = "\"";
                    } else {
                        var isnum = /^\d+$/.test(index);
                        coteIndex = !isnum && index != "null" && !(typeof index === "boolean") ? "\"" : "";
                    }
                    var indexFilter = coteIndex + index + coteIndex;
                    var click = "$(\"#" + id + "\").data(\"kendoOmfPanelBar\").filtreElements(\"" + dataItem.field + "\"," + indexFilter + ", event)";
                    var tagSum = dataItem.sumField != null ? "<span class='badge my-badge'>" + kendo.toString(value.sum, 'n2') + "</span>" : "";
                    var tagCount = "<span class='badge badge-count my-badge'>" + value.count + "</span>";
                    return [{ "encoded": false, "cssClass": cssClass, "text": "<div  " + borderColor + " onclick='" + click + "'>" + (dataItem.dsGetLb != null && dataItem.dsGetLb.get(index) != null ? dataItem.dsGetLb.get(index).lb : (index != "null" ? index : 'Aucun')) + "<span class='content-my-badge'>" + tagSum + tagCount + "</span>" + "</div>" }];
                });

                dataItem.items = items;
            }

            element = that.element;
            options = that.options;

            that.setOptions({ dataSource: that.options.dataSource });

            that.countData = that.options.dataSourceWidget.data().length;
            that.countView = that.options.dataSourceWidget.view().length;

            if ($("#" + id + "-btn-all-elements").length == 0) {
                var btnAllElements = $("<button class='btn btn-default btn-all-elements' id='" + id + "-btn-all-elements'>Tous<span class='badge desc-count'>" + that.countView + " / " + that.countData + "</span></button>");
                btnAllElements.click(function () {
                    that.selectedItems = [];
                    that.selectedFields = [];
                    that.options.dataSourceWidget.filter({});
                    if (that.lvGrouped != null)
                        that.setDataSourceLvGrouped();
                });
                btnAllElements.insertBefore($(that.element[0]));
            }
            else {
                $("#" + id + "-btn-all-elements .desc-count").text(that.countView + " / " + that.countData);
            }

            $(that.element[0]).find(".m-input-filter-items").each(function () {
                var id = $(this).parent().find(".m-title-panelbar-item").text();
                $(this).parent().parent().find("ul.k-group.k-panel").first().attr("id", id);
            });

            $(that.element[0]).find(".m-input-filter-items").keyup(function () {
                var val = $(this).val();
                var ulGroupPanel = $(this).parent().parent().find("ul.k-group.k-panel").first();
                HightLightSearchWidget(ulGroupPanel.attr("id"), val);
            }).click(function (e) {
                e.preventDefault();
                e.stopPropagation();
            });


            $(that.element[0]).find(".k-group").each(function () {
                var maxWidth = 0;
                $(this).find(".badge-count").each(function () {
                    var w = $(this).width();
                    if (w > maxWidth)
                        maxWidth = w;
                });

                $(this).find(".badge-count").width(maxWidth);
            });
        },
        countView: 0,
        countData: 0,
        selectedItems: [],
        selectedFields: [],
        lvGrouped: null,
        options: {
            name: "OmfPanelBar",
        },
        isGuid: function (value) {
            var regex = /[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/i;
            var match = regex.exec(value);
            return match != null;
        },
        getIds: function () {
            var that = this;
            var view = that.options.dataSourceWidget.view();
            return $.map(view, function (e) { return e.id; });
        },
        getOperatorByField: function (field) {
            var that = this;
            var filters = that.options.dataSourceWidget.filter();
            if (typeof filters !== "undefined")
                filters = filters.filters;
            if (typeof filters !== "undefined") {
                var index = $.map(filters, function (e) { return e.fld; }).indexOf(field);
                if (index != -1)
                    return filters[index].op;
            }
            return "eq";
        },
        clear: function () {
            var that = this;
            that.selectedItems = [];
            that.selectedFields = [];
            that.options.dataSourceWidget.filter({});
            that.setDataSourceLvGrouped();
        },
        removeFilteredItem: function (uid) {
            var that = this;
            var dataItem = that.lvGrouped.dataSource.getByUid(uid);
            var index = dataItem.index;
            var f = [];
            if (typeof that.options.dataSourceWidget.filter() !== "undefined")
                f = that.options.dataSourceWidget.filter().filters;
            f.splice(index, 1);
            var idxField = that.selectedFields.indexOf(dataItem.field);
            if (idxField != -1) {
                that.selectedFields.splice(idxField, 1);
                that.selectedItems.splice(idxField, 1);
            }

            that.options.dataSourceWidget.filter({ logic: "and", filters: f });
            that.setDataSourceLvGrouped();
        },
        setDataSourceLvGrouped: function () {
            var that = this;
            var data = [];
            var f = [];
            if (typeof that.options.dataSourceWidget.filter() !== "undefined")
                f = that.options.dataSourceWidget.filter().filters;
            $.each(f, function (idx, ele) {
                var exist = false;
                for (var i = 0 ; i < that.options.dataSource.length; i++) {
                    var dataItem = that.options.dataSource[i];
                    if (dataItem.field == ele.fld) {
                        var text = ele.value;
                        if (dataItem.dsGetLb != null && dataItem.dsGetLb.get(ele.value) != null)
                            text = dataItem.dsGetLb.get(ele.value).lb;
                        else if (ele.value == null)
                            text = "Aucun";
                        var op = ele.op == "neq" ? "<>" : "=";
                        data.push({ lb: dataItem.defaultText, text: text, field: ele.fld, index: idx, operator: op, neq: ele.op == "neq" });
                        exist = true;
                        break;
                    }
                }
            });
            that.lvGrouped.setDataSource(new kendo.data.DataSource({ data: data }));
        }
    });

    kendo.ui.plugin(OmfPanelBar);
})(jQuery);