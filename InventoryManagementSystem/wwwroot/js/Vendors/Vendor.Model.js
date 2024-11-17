/// <reference path="../knockout.js" />

var VendorModel = function (item) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.id || 0);
    self.Name = ko.observable(item.name || '');
    self.Contact = ko.observable(item.contact || '');
    self.Address = ko.observable(item.address || '');
}