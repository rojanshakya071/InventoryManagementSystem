/// <reference path="../knockout.js" />


var SalesMasterVM = function (item, parent) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.id || 0);
    self.CustomerId = ko.observable(item.customerId || '');
    self.SalesDate = ko.observable(item.salesDate || '');
    self.CustomerName = ko.observable(item.customerName || '');
    self.InvoiceNumber = ko.observable(item.invoiceNumber || 0);
    self.Discount = ko.observable(item.discount || 0);
    self.Sales = ko.observableArray((item.sales || []).map(function (item) {
        return new SalesDetailVM(item, self);
    }));

    // Computed property for billAmount
    self.BillAmount = ko.computed(() => {
        return self.Sales().reduce((sum, item) => sum + parseFloat(item.Amount() || 0), 0);
    });

    self.CalculatePercentage = ko.computed(() => {
        return (parseFloat(self.Discount() || 0) / 100) * self.BillAmount();
    });

    self.NetAmount = ko.computed(() => {
        return self.BillAmount() - self.CalculatePercentage();
    });
};


var SalesDetailVM = function (item, parent) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.id || 0);
    self.ItemId = ko.observable(item.itemId || '');
    self.Unit = ko.observable(item.unit || '');
    self.AvailableQuantity = ko.observable(item.availableQuantity || 0);
    self.Quantity = ko.observable(item.quantity || 0);
    self.Price = ko.observable(item.price || 0);
    self.Amount = ko.computed(function () {
        return self.Quantity() * self.Price();
    });

    // Subscribe to changes in ItemId to update Unit, quantity
    self.ItemId.subscribe(function (newValue) {
        var selectedItem = parent.ItemsNameList().find(function (item) {
            return item.Id() === newValue;
        });

        if (selectedItem) {
            self.Unit(selectedItem.Unit());
            self.AvailableQuantity(selectedItem.Quantity()); 
        } else {
            self.Unit('');
            self.AvailableQuantity(0);
        }
    });

};




var CustomerNameVM = function (item) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.id || 0);
    self.CustomerName = ko.observable(item.customerName || '');
};

var ItemNameVM = function (item) {
    var self = this;
    item = item || {};
    self.Id = ko.observable(item.id || 0);
    self.ItemName = ko.observable(item.itemName || '');
    self.Unit = ko.observable(item.unit || 0);
    self.Quantity = ko.observable(item.quantity || 0);
};