﻿$("#menuitemuser").click(function () {
    $("#PermissionsResults").html("");
    $("#searchinput").html("User <span class='caret'></span>");
    $("#searchterm").prop("type", "text").prop("name", "Username").prop("placeholder", "Username").val("").focus();
});

$("#menuitememail").click(function () {
    $("#PermissionsResults").html("");
    $("#searchinput").html("Email <span class='caret'></span>");
    $("#searchterm").prop("type", "email").prop("name", "emailAddress").prop("placeholder", "Email Address").val("").focus();
});

$("#menuitemurl").click(function () {
    $("#PermissionsResults").html("");
    $("#searchinput").html("URL <span class='caret'></span>");
    $("#searchterm").prop("type", "text").prop("name", "pageUrl").prop("placeholder", "Page URL").val("").focus();
});

$("#menuitemenodeid").click(function () {
    $("#PermissionsResults").html("");
    $("#searchinput").html("Id <span class='caret'></span>");
    $("#searchterm").prop("type", "text").prop("name", "nodeId").prop("placeholder", "Node Id").val("").focus();
});

$(function () {
    if ($("#searchterm").length) $("#searchterm").focus();

    $("#tree").fancytree({
        source: {
            url: $("#apppath").html() + "/Permissions/PopTreeRootResult",
            cache: false,
            data: { UserId: $("#userId").html() }
        },
        lazyLoad: function (event, data) {
            var node = data.node;
            data.result = {
                url: $("#apppath").html() + "/Permissions/PopTreeChildResult",
                cache: false,
                data: { RootId: node.key, UserId: data.node.data.UserId }
            };
        },
        selectMode: 2,
        checkbox: true,
        autoScroll: true,
        generateIds: true,
        idPrefix: "",
        loadChildren: function (event, data) {
            if (data.node.hasChildren()) {
                data.node.folder = true;
            };
        },
        select: function (event, data) {
            $.getJSON($("#apppath").html() + "/Permissions/ChangePermissionsResult", { PageId: data.node.key, UserId: data.node.data.UserId, selected: data.node.selected, PageName: data.node.title }, function (returnData) {
                if (!returnData) {
                    if (data.node.selected == true) {
                        alert("Error setting permissions");
                        data.node.selected = false;
                    }
                    else {
                        alert("Error removing permissions");
                        data.node.selected = true;
                    }
                }
            });
        }
    });
});

$("#finduser").click(function () {
    if ($("#searchterm").prop("name") == "emailAddress") {
        var email = $("#searchterm").val();
        $.getJSON($("#apppath").html() + "/Permissions/CheckDestinationUser", { EmailAddress: email }, updateFields);
    }
    else {
        var user = $("#searchterm").val();
        $.getJSON($("#apppath").html() + "/Permissions/CheckDestinationUser", { UserName: user }, updateFields);
    };
});

$("#searchterm").keypress(function (e) {
    if (e.which == 13) {
        $("#finduser").trigger("click");
    }
});

updateFields = function (data) {
    if (data == false) {
        //alert("Error - Unable to find user");
        $("#result").html("<p><strong>User not found</strong></p>");
        $("#copyPerms").attr("disabled", "disabled");
    } else {
        $("#result").html("<p id='sourceId' class='" + data.UserId + "'><strong>Id:</strong> " + data.UserId + "</p><p><strong>Name:</strong> " + data.FullName + "</p><p><strong>Email:</strong> " + data.EmailAddress + "</p><p><strong>Logon ID:</strong> " + data.UserName + "</p>");
        $("#copyPerms").removeAttr("disabled");
    }
}

$("#copyPerms").click(function () {
    var token = $('[name=__RequestVerificationToken]').val();
    $.ajax({
        url: $("#apppath").html() + "/Permissions/CopyPermissionsForUser",
        type: "POST",
        data: { __RequestVerificationToken: token, sourceId: $("#sourceId").prop("class"), targetId: $("#userId").html() },
        dataType: "json",
        success: function (json) {
            if (json.isRedirect) {
                window.location.href = json.redirectUrl + "/Index/" + $("#userId").html();
            }
        }
    });
});

$("#myModalClose").click(function () {
    $("#searchterm").val("");
    $("#result").empty();
});

$(document).on("change", ".haspermission", function () {
    var chk = $(this);

    var apppath = $("#apppath").html();
    var pageid = $("#pageid").html();
    var pagename = $("#pagename").html();
    var userid = chk.val();
    var selected = chk.is(":checked");

    //console.log("Changing " + chk.val() + ":");
    //console.log("apppath = " + apppath);
    //console.log("pageid = " + pageid);
    //console.log("pagename = " + pagename);
    //console.log("userid = " + userid);
    //console.log("selected = " + selected);

    $.getJSON(apppath + "/Permissions/ChangePermissionsResult", { PageId: pageid, UserId: userid, selected: selected, PageName: pagename }, function (returnData) {
        if (!returnData) {
            if (selected === true) {
                alert("Error setting permissions");
                selected = false;
            }
            else {
                alert("Error removing permissions");
                selected = true;
            }
        }
    });
});

//$("#PagesWithoutAuthor").click(function () {
//    var btn = $(this);
//    btn.prop("disabled", true);
//    var dest = $("#UnauthordPermissions");
//    var otherdest = $("#PermissionsResults");
//    dest.html("<img src=\"../Content/ajax-loader.gif\" class=\"loaderimg\" alt=\"Please wait...\" />");
//    otherdest.html("");
//    $.get($("#apppath").html() + "/Tools/CheckPagesWithoutAuthor/", function (data) {
//        dest.html(data);
//        btn.prop("disabled", false);
//    });
//    return false;
//});

$("#lookupPermissions").click(function () {
    var btn = $(this);
    btn.prop("disabled", true);
    var dest = $("#PermissionsResults");
    var otherdest = $("#UnauthordPermissions");
    dest.html("<img src=\"../Content/ajax-loader.gif\" class=\"loaderimg\" alt=\"Please wait...\" />");
    otherdest.html("");
    var searchTerm = $("#searchterm").prop("name");

    switch (searchTerm) {
        case "emailAddress":
            var email = $("#searchterm").val();
            if (email.length === 0) {
                dest.html(errorMessage("Please enter an Email Address"));
                btn.prop("disabled", false);
                return false;
            }
            $.get($("#apppath").html() + "/Tools/CheckUserPermissions/", { EmailAddress: email }, function (data) {
                dest.html(data);
                btn.prop("disabled", false);
            });
            break;

        case "Username":
            var user = $("#searchterm").val();
            if (user.length === 0) {
                dest.html(errorMessage("Please enter a Username"));
                btn.prop("disabled", false);
                return false;
            }
            $.get($("#apppath").html() + "/Tools/CheckUserPermissions/", { UserName: user }, function (data) {
                dest.html(data);
                btn.prop("disabled", false);
            });
            break;

        case "pageUrl":
            var url = $("#searchterm").val();
            if (url.length === 0) {
                dest.html(errorMessage("Please enter a Page Url"));
                btn.prop("disabled", false);
                return false;
            }
            $.get($("#apppath").html() + "/Tools/CheckPagePermissions/", { url: url }, function (data) {
                dest.html(data);
                btn.prop("disabled", false);
            });
            break;
        default:
            dest.html("");
            btn.prop("disabled", false);
            break;
    }
    return false;
});

$("#lookupUserPermissions").click(function () {
    var btn = $(this);
    btn.prop("disabled", true);
    var dest = $("#PermissionsResults");
    var otherdest = $("#UnauthordPermissions");
    dest.html("<img src=\"../Content/ajax-loader.gif\" class=\"loaderimg\" alt=\"Please wait...\" />");
    otherdest.html("");
    var searchTerm = $("#searchterm").prop("name");

    switch (searchTerm) {
        case "emailAddress":
            var email = $("#searchterm").val();
            if (email.length === 0) {
                dest.html(errorMessage("Please enter an Email Address"));
                btn.prop("disabled", false);
                return false;
            }
            $.get($("#apppath").html() + "/Tools/LookupUserPermissions/", { EmailAddress: email }, function (data) {
                dest.html(data);
                btn.prop("disabled", false);
            });
            break;

        case "Username":
            var user = $("#searchterm").val();
            if (user.length === 0) {
                dest.html(errorMessage("Please enter a Username"));
                btn.prop("disabled", false);
                return false;
            }
            $.get($("#apppath").html() + "/Tools/LookupUserPermissions/", { UserName: user }, function (data) {
                dest.html(data);
                btn.prop("disabled", false);
            });
            break;

        default:
            dest.html("");
            btn.prop("disabled", false);
            break;
    }
    return false;
});

$("#lookupPagePermissions").click(function() {
    var btn = $(this);
    btn.prop("disabled", true);
    var dest = $("#PermissionsResults");
    var otherdest = $("#UnauthordPermissions");
    dest.html("<img src=\"../Content/ajax-loader.gif\" class=\"loaderimg\" alt=\"Please wait...\" />");
    otherdest.html("");
    var searchTerm = $("#searchterm").prop("name");

    switch (searchTerm) {
        case "pageUrl":
            var url = $("#searchterm").val();
            if (url.length === 0) {
                dest.html(errorMessage("Please enter a URL"));
                btn.prop("disabled", false);
                return false;
            }
            $.get($("#apppath").html() + "/Tools/LookupPagePermissions/", { Url: url }, function (data) {
                dest.html(data);
                btn.prop("disabled", false);
            });
            break;

        case "nodeId":
            var nodeid = $("#searchterm").val();
            if (nodeid.length === 0) {
                dest.html(errorMessage("Please enter a Node Id"));
                btn.prop("disabled", false);
                return false;
            }
            $.get($("#apppath").html() + "/Tools/LookupPagePermissions/", { NodeId: nodeid }, function (data) {
                dest.html(data);
                btn.prop("disabled", false);
            });
            break;

        default:
            dest.html("");
            btn.prop("disabled", false);
            break;
    }

    return false;
});

$("#lookupPageAuthors").click(function () {
    var dest = $("#PermissionsResults");
    var otherdest = $("#UnauthordPermissions");
    dest.html("");
    otherdest.html("");
    var btn = $(this);
    var url = $("#searchterm").val();

    if (url.length === 0) {
        dest.html(errorMessage("Please enter a URL"));
        return false;
    }

    url = encodeURIComponent(url);
    btn.prop("disabled", true);
    dest.html("<img src=\"Content/ajax-loader.gif\" class=\"loaderimg\" alt=\"Please wait...\" />");

    $.get($("#apppath").html() + "/PageAuthor/CheckPagePermissions/", { url: url }, function (data) {
        dest.html(data);
        btn.prop("disabled", false);
    });

    return false;
});

$("#lookupInboundLinks").click(function() {
    var dest = $("#PermissionsResults");
    var otherdest = $("#UnauthordPermissions");
    dest.html("");
    otherdest.html("");
    var btn = $(this);
    var url = $("#searchterm").val();

    if (url.length === 0) {
        dest.html(errorMessage("Please enter a URL"));
        return false;
    }

    url = encodeURIComponent(url);
    btn.prop("disabled", true);
    dest.html("<img src=\"../Content/ajax-loader.gif\" class=\"loaderimg\" alt=\"Please wait...\" />");

    $.get($("#apppath").html() + "/Tools/FindInboundLinks/", { url: url }, function (data) {
        dest.html(data);
        btn.prop("disabled", false);
    });

    return false;
});

$("#url").keypress(function (e) {
    if (e.which == 13) {
        $("#checkpage").trigger("click");

        if ($("#searchterm").length) $("#searchterm").blur();
    }
});

$("#searchterm").keypress(function (e) {
    if (e.which == 13) {
        if ($("#lookupPagePermissions").length) $("#lookupPagePermissions").trigger("click");
        if ($("#lookupPermissions").length) $("#lookupPermissions").trigger("click");
        if ($("#lookupInboundLinks").length) $("#lookupInboundLinks").trigger("click");

        if ($("#searchterm").length) $("#searchterm").blur();
    }
});

function lookupallauthors(elem) {
    var btnId = $(elem).data("url");
    $("#menuitemurl").click();
    $("#searchterm").val(btnId);
    $("#lookupPermissions").click();
    return false;
};

function lookupauthorpages(elem) {
    var btnId = $(elem).data("id");
    $("#menuitemuser").click();
    $("#searchterm").val(btnId);
    $("#lookupPermissions").click();
    return false;
};

function transfertouserpermissions(elem) {
    var url = '@Url.Action(\"TransferToUserPermissions\", \"Tools\", new { UserName: " + "fred" + "})';
    window.location = url;

}

function errorMessage(msg) {
    var rtnMsg = "<div class" + "=\"alert alert-danger\"><p class=\"highlight\">";
    rtnMsg = rtnMsg + msg;
    rtnMsg = rtnMsg + "</p></div>";
    return rtnMsg;
};