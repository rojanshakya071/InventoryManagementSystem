/// <reference path="../knockout.js" />

var CustomerModel = function (item) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.id || 0); 
    self.FullName = ko.observable(item.fullName || '');
    self.ContactNo = ko.observable(item.contactNo || '');
    self.Address = ko.observable(item.address || '');
}