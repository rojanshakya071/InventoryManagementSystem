/// <reference path="purchases.model.js" />

const mode = {
    create: 1,
    update: 2
}
var PurchasesController = function () {

    var self = this;
    const baseUrl = '/api/PurchaseReportAPI';
    self.NewPurchase = ko.observable(new PurchaseMasterVM({}, self));
    self.CurrentPurchase = ko.observableArray([]);
    self.SelectedPurchase = ko.observable(new PurchaseMasterVM({}, self));
    self.IsUpdated = ko.observable(false);
    self.mode = ko.observable(mode.create);
    self.PurchaseToBeDelete = ko.observable();
    self.VendorNameList = ko.observableArray([]);
    self.ItemsNameList = ko.observableArray([]);
    self.venors = ko.observable(new VendorsNameVM());

    self.GetData = function () {
        ajax.get(baseUrl + "/GetAll").then(function (result) {
            console.log(result);
            self.CurrentPurchase(result.map(item => new PurchaseMasterVM(item, self)));
        });
    }
    self.GetData();

    self.GetVendorNameList = function () {
        ajax.get(baseUrl + "/GetAllVendor").then(function (result) {
            self.VendorNameList(result.map(item => new VendorsNameVM(item)));
        });
    }
    self.GetVendorNameList();

    self.GetItemsName = function () {
        ajax.get(baseUrl + "/GetItemsName").then(function (result) {
            self.ItemsNameList(result.map(item => new ItemsNameVM(item)));
        });
    }
    self.GetItemsName();

    self.AddItem = function () {
        if (self.IsUpdated()) {
            self.SelectedPurchase().Purchases.push(new PurchaseDetailsVM({}, self));
        } else {
            self.NewPurchase().Purchases.push(new PurchaseDetailsVM({}, self));
        }
    }

    self.RemoveItem = function (item) {
        if (self.IsUpdated()) {
            self.SelectedPurchase().Purchases.remove(item);
        } else {
            self.NewPurchase().Purchases.remove(item);
        }
    }

    self.DeletePurchase = function (model) {
        self.PurchaseToBeDelete(model);
        setTimeout(function () {
            $('#deleteConfirmModal').modal('show');
        }, 100);
    };

    self.validate = function () {
        var isvalid = true;
        var purchase = self.IsUpdated() ? self.SelectedPurchase : self.NewPurchase;

        if (!purchase().VendorId()) {
            toastr.error("Please select Vendor ");
            isvalid = false;
            return;
        }
        if (purchase().InvoiceNumber() < 0 || purchase().InvoiceNumber() == '') {
            toastr.error("Please Invoice is required and can't be negative ");
            isvalid = false;
            console.log("InvoiceNumber", isvalid)
            return;
        }
        if (purchase().Discount() < 0 || purchase().Discount() > 100 || purchase().Discount() === '') {
            toastr.error("Discount must be between 0 and 100");
            isvalid = false;
            console.log("dis", isvalid);
            return;
        }
        if (purchase().Purchases().length == 0) {
            toastr.error("At least one item is required.");
            isvalid = false;
            console.log("sale Len", isvalid)
            return;
        }

        purchase().Purchases().forEach((item, index) => {

            if (!item.ItemId()) {
                toastr.error("You must have to choose one item");
                isvalid = false;
                console.log("ItemID", isvalid)
                return;
            }
            if (item.Quantity() == '' || item.Quantity() <= 0) {
                toastr.error("Quantity is required and must be greater than 0");
                isvalid = false;
                console.log("quantity", isvalid)
                return;
            }
            if (!item.Amount() || item.Amount() <= 0) {
                toastr.error("Amount is required and must be greater than 0");
                isvalid = false;
                console.log("Price", isvalid)
                return;
            }
        });
        return isvalid;
    }

    self.AddPurchase = function () {
        var isvalid = self.validate();
        var purchaseData = ko.toJS(self.IsUpdated() ? self.SelectedPurchase : self.NewPurchase);
        if (isvalid) {
            switch (self.mode()) {
                case mode.create:
                    ajax.post(baseUrl + "/Create", JSON.stringify(purchaseData))
                        .done(function (result) {
                            if (result.success) {
                                console.log("Data received", result);
                                self.CurrentPurchase.push(new PurchaseMasterVM(result, self));
                                self.ResetForm();
                                self.GetData();
                                $('#orderModal').modal('hide');
                                Swal.fire("Item Added Successfully");
                            }
                            else {
                                alert(result.message);
                            }
                        })
                        .fail(function (err) {
                            console.log("Error adding order:", err);
                        });
                    break;
                case mode.update:
                    ajax.put(baseUrl + "/Update", JSON.stringify(purchaseData))
                        .done(function (result) {
                            var updatedOrder = new PurchaseMasterVM(result, self);
                            var index = self.CurrentPurchase().findIndex(function (item) {
                                return item.Id() === updatedOrder.Id();
                            });
                            if (index >= 0) {
                                self.CurrentPurchase.replace(self.CurrentPurchase()[index], updatedOrder);
                            }
                            self.ResetForm();
                            self.GetData();
                            $('#orderModal').modal('hide');
                            toastr.success("Purchase Updated Successfully");
                        })
                        .fail(function (err) {
                            console.log("Error updating order:", err);
                        });
                    break;
            }
        }
    };

    self.SelectPurchase = function (model) {
        self.SelectedPurchase(model);
        self.IsUpdated(true);
        self.mode(mode.update);
        $('#orderModal').modal('show');
    }

    self.ResetForm = function () {
        self.NewPurchase(new PurchaseMasterVM({}, self));
        self.SelectedPurchase(new PurchaseMasterVM({}, self));
        self.IsUpdated(false);
        self.mode(mode.create);
        //self.AddItem();
    }

    self.calculateTotal = function () {
        self.totalAmount.notifySubscribers();  // Recalculate the total amount
    };

    self.CloseModel = function () {
        self.ResetForm();
    }

    self.ResetForm();

    self.confirmDelete = function () {
        debugger
        var model = self.PurchaseToBeDelete();
        if (model) {
            ajax.delete(baseUrl + "/Delete?id=" + model.Id())
                .done((result) => {

                    $('#deleteConfirmModal').modal('hide');
                    toastr.success("Deleted Successfully")
                    self.GetData();
                })
                .fail((err) => {
                    console.log(err);
                    $('#deleteConfirmModal').modal('hide');
                });
        }
        else {
            toastr.error("Item Used in sales table")
        }
    };

   
}

var ajax = {
    get: function (url) {
        return $.ajax({
            method: "GET",
            url: url,
            async: false,
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
            data: data
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
};
