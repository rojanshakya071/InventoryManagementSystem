/// <reference path="../knockout.js" />

var ItemModel = function (item) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.id || 0); 
    self.Name = ko.observable(item.name || '');
    self.Unit = ko.observable(item.unit || '');
    self.Category = ko.observable(item.category || '');
}
