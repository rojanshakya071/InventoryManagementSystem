/// <reference path="../knockout.js" />

var itemmodel = function (item) {
    var self = this;
    item = item || {};
    self.id = ko.observable(item.id || 0);
    self.itemId = ko.observable(item.itemId || 0);
    self.itemName = ko.observable(item.itemName || '');
    self.quantity = ko.observable(item.quantity || 0);
}