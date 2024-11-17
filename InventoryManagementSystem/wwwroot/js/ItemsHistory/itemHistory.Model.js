/// <reference path="../knockout.js" />

var itemhistorymodel = function (item) {
    var self = this;
    item = item || {};
    self.id = ko.observable(item.id || 0);
    self.itemId = ko.observable(item.itemId || 0);
    self.itemName = ko.observable(item.itemName || '');
    self.quantity = ko.observable(item.quantity || 0);
    self.transDate = ko.observable(item.transDateFormatted || '');
    self.stockCheckOut = ko.observable(item.stockCheckOutText || 0);
    self.transactionType = ko.observable(item.transactionTypeText || 0);
}