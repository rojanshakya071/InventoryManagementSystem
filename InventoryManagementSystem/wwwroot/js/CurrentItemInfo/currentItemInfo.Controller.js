/// <reference path="../knockout.js" />
/// <reference path="currentiteminfo.model.js" />

var currentitemcontroller = function () {
    var self = this;
    const baseUrl = "/api/CurrentItemsInfoAPI";
    self.CurrentItemList = ko.observableArray([]);




    self.getData = function () {
        ajax.get(baseUrl).then(function (result) {
            self.CurrentItemList(result.map(item => new itemmodel(item)));
        });
    }

    self.getData();

}
var ajax = {
    get: function (url) {
        return $.ajax({
            method: "GET",
            url: url,
            async: false
        })
    },
    post: function (url, data) {
        return $.ajax({
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            method: "POST",
            url: url,
            data: (data)
        });
    },
    put: function (url, data) {
        return $.ajax({
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            method: "PUT",
            url: url,
            data: data
        });
    },
    delete: function (route) {
        return $.ajax({
            method: "DELETE",
            url: route,
        });
    }
}