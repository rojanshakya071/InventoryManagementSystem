/// <reference path="../knockout.js" />

var PurchaseMasterVM = function (item, parent) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.id || 0);
    self.VendorId = ko.observable(item.vendorId || 0);
    self.VendorName = ko.observable(item.vendorName || '');
    self.InvoiceNumber = ko.observable(item.invoiceNumber || 0);
    self.Discount = ko.observable(item.discount || 0);
    self.Purchases = ko.observableArray((item.purchases || []).map(function (item) {
        return new PurchaseDetailsVM(item, self);
    }));
    self.BillAmount = ko.computed(() => {
        return self.Purchases().reduce((sum, item) => sum + parseFloat(item.Price() || 0), 0);
    });

    self.CalculatePercentage = ko.computed(() => {
        return (parseFloat(self.Discount() || 0) / 100) * self.BillAmount();
    });

    self.NetAmount = ko.computed(() => {
        return self.BillAmount() - self.CalculatePercentage();
    });
}



var PurchaseDetailsVM = function (item, parent) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.id || 0);
    self.ItemId = ko.observable(item.itemId || 0);
    self.Unit = ko.observable(item.unit || '');
    self.Quantity = ko.observable(item.quantity || 0);
    self.Amount = ko.observable(item.amount || '');
    self.Price = ko.computed(function () {
        return self.Quantity() * self.Amount();
    });
    self.ItemId.subscribe(function (newValue) {
        var selectedItem = parent.ItemsNameList().find(function (item) {
            return item.Id() === newValue;
        });

        if (selectedItem) {
            self.Unit(selectedItem.Unit());
        } else {
            self.Unit('');
        }
    });
}


var VendorsNameVM = function (item) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.id || 0);
    self.VendorName = ko.observable(item.vendorName || '');
}

var ItemsNameVM = function (item) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.id || 0);
    self.ItemName = ko.observable(item.itemName || '');
    self.Unit = ko.observable(item.unit || '');
    
}