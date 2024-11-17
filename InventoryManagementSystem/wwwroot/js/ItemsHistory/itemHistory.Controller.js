/// <reference path="itemhistory.model.js" />
/// <reference path="../knockout.js" />
var itemhistorycontroller = function () {
    var self = this;
    const baseUrl = "/api/ItemsInfoHistoryAPI";
    self.ItemHistoryList = ko.observableArray([]);

    self.searchQuery = ko.observable("");
    self.selectedTransactionType = ko.observable("");
    self.searchArrayList = ko.observableArray([]);
    self.currentPage = ko.observable(1);
    self.itemsPerPage = ko.observable(10);
    self.totalPages = ko.computed(function () {
        return Math.ceil(self.ItemHistoryList().length / self.itemsPerPage());
    });

    self.pagedItems = ko.computed(function () {
        var startIndex = (self.currentPage() - 1) * self.itemsPerPage();
        return self.ItemHistoryList.slice(startIndex, startIndex + self.itemsPerPage());
    });

    // Navigation methods
    self.nextPage = function () {
        if (self.currentPage() < self.totalPages()) {
            self.currentPage(self.currentPage() + 1);
        }
    };

    self.previousPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
        }
    };
    self.goToPage = function (page) {
        self.currentPage(page);
    };

    self.GetDatas = function () {
        var search = self.searchQuery();
        var transactionType = self.selectedTransactionType();
        var url = baseUrl + "?search=" + encodeURIComponent(search);
        if (transactionType !== "") {
            url += "&transactionType=" + transactionType;
        }
        ajax.get(url).then(function (result) {
            self.ItemHistoryList(result.map(item => new itemhistorymodel(item)));
            self.currentPage(1);
        });
    };

    self.search = function () {
        self.GetDatas();
    };

    self.generateReport = function () {
        var search = self.searchQuery();
        var transactionType = self.selectedTransactionType();
        var url = baseUrl + "/GenerateReport?search=" + encodeURIComponent(search);
        if (transactionType !== "") {
            url += "&transactionType=" + transactionType;
        }
        window.location.href = url;
    };

    self.GetDatas();
}

var ajax = {
    get: function (url) {
        return $.ajax({
            method: "GET",
            url: url,
            async: false
        });
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
            url: route
        });
    }
}
