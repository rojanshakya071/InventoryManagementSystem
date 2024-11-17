/// <reference path="salesreport.model.js" />

const mode = {
    create: 1,
    update: 2
};

var SalesReportController = function () {
    var self = this;
  
    const baseUrl = "/api/SalesReportAPI";
    self.CurrentOrder = ko.observableArray([]);
    self.CustomersNameList = ko.observableArray([]);
    self.ItemsNameList = ko.observableArray([]);
    self.IsUpdated = ko.observable(false);
    self.SelectedOrder = ko.observable(new SalesMasterVM({}, self));//self typically refers to the parent ViewModel or controller (SalesReportController).
    self.NewOrder = ko.observable(new SalesMasterVM({},self));
    self.mode = ko.observable(mode.create);
    self.salesToDelete = ko.observable();

    // Fetch Data From Server 
    self.getData = function () {
        ajax.get(baseUrl + "/GetAll").then(function (result) {
            self.CurrentOrder(result.map(item => new SalesMasterVM(item,self)));
        });
    }
    self.getData();

    self.getCustomersName = function () {
        ajax.get(baseUrl + "/GetAllCustomers").then(function (result) {
            self.CustomersNameList(result.map(item => new CustomerNameVM(item)));
        });
    }
    self.getCustomersName();

    self.getItemsName = function () {
        ajax.get(baseUrl + "/GetItemsName").then(function (result) {
            self.ItemsNameList(result.map(item => new ItemNameVM(item)));
        });
    };

    self.getItemsName(); 
   
    self.validate = function () {
        var isvalid = true;
        var order = self.IsUpdated() ? self.SelectedOrder : self.NewOrder;
        console.log(ko.toJS(order));
        if (order().SalesDate() == '') {
            toastr.error("Please select Date");
            isvalid = false;
            return;
        }
        if (!order().CustomerId() ) {
            toastr.error("Please select Customer ");
            isvalid = false;
            console.log("customerId", isvalid)
            return;
        }
        if (order().InvoiceNumber() < 0 || order().InvoiceNumber() == '') {
            toastr.error("Please Invoice is required and can't be negative ");
            isvalid = false;
            console.log("InvoiceNumber", isvalid)
            return;
        }
        if (order().Discount() < 0 || order().Discount() > 100 || order().Discount() === '') {
            toastr.error("Discount must be between 0 and 100");
            isvalid = false;
            console.log("dis", isvalid);
            return;
        }
        if (order().Sales().length == 0) {
            toastr.error("At least one item is required.");
            isvalid = false;
            console.log("sale Len", isvalid)
            return;
        }
        
        order().Sales().forEach((item, index) => {
            
            if (!item.ItemId() ) {
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
            if (!item.Price() || item.Price() <= 0) {
                toastr.error("Price is required and must be greater than 0");
                isvalid = false;
                console.log("Price", isvalid)
                return;
            }
        });
        return isvalid;
    }

    self.AddOrder = function () {

        var isvalid = self.validate();
        
        var orderData = ko.toJS(self.IsUpdated() ? self.SelectedOrder : self.NewOrder);
        if (isvalid == true) {
            switch (self.mode()) {
                case mode.create:
                    ajax.post(baseUrl + "/Create", JSON.stringify(orderData))
                        .done(function (result) {
                            if (result.success) {
                                console.log("Data received", result);
                                self.CurrentOrder.push(new SalesMasterVM(result, self));
                                self.resetForm();
                                self.getData();
                                $('#orderModal').modal('hide');
                                Swal.fire("Item Added Successfully");
                            }
                            else {
                                toastr.error(result.message)
                            }
                        })
                        .fail(function (err) {
                            console.log("Error adding order:", err);
                        });
                    break;
                case mode.update:

                    ajax.put(baseUrl + "/Update", JSON.stringify(orderData))
                        .done(function (result) {
                            var updatedOrder = new SalesMasterVM(result, self);
                            var index = self.CurrentOrder().findIndex(function (item) {
                                return item.Id() === updatedOrder.Id();
                            });
                            if (index >= 0) {
                                self.CurrentOrder.replace(self.CurrentOrder()[index], updatedOrder);
                            }
                            self.resetForm();
                            self.getData();
                            $('#orderModal').modal('hide');
                            toastr.success("Sale Updated Successfully");
                        })
                        .fail(function (err) {
                            console.log("Error updating order:", err);
                        });
                    break;
            }
        }
        
    };

    self.DeleteSales = function (model) {
        self.salesToDelete(model);
        setTimeout(function () {
            $('#deleteConfirmModal').modal('show');
        }, 100);
    };

    self.confirmDelete = function () {
        var model = self.salesToDelete();
        if (model) {
            ajax.delete(baseUrl + "/Delete?id=" + model.Id())
                .done((result) => {
                    self.CurrentOrder.remove(model);
                    $('#deleteConfirmModal').modal('hide');
                    toastr.success("deleted Successfully")
                })
                .fail((err) => {
                    console.log(err);
                    $('#deleteConfirmModal').modal('hide');
                });
        }
    };


    // Delete Product
    self.DeleteProduct = function (model) {
        ajax.delete(baseUrl + "/Delete?id=" + model.Id())
            .done((result) => {
                self.CurrentOrder.remove(function (item) {
                    return item.Id() === model.Id();
                });
            }).fail((err) => {
                console.log(err);
            });
    };

    self.SelectOrder = function (model) {
        self.SelectedOrder(model);
        self.IsUpdated(true);
        self.mode(mode.update);
        $('#orderModal').modal('show');
    }

    self.CloseModel = function () {
        self.IsUpdated(false);
        self.resetForm();

    }

    self.resetForm = function () {
        self.NewOrder(new SalesMasterVM({}, self));
        self.SelectedOrder(new SalesMasterVM({}, self));
        self.mode(mode.create);
        self.IsUpdated(false);
       // self.AddItem(); // Add an initial empty item
    }

    // Remove Item
    self.RemoveItem = function (item) {
        if (self.IsUpdated()) {
            self.SelectedOrder().Sales.remove(item);
        } else {
            self.NewOrder().Sales.remove(item);
        }
    };

     //Add Item
    self.AddItem = function () {
        if (self.IsUpdated()) {
            self.SelectedOrder().Sales.push(new SalesDetailVM({},self));
        } else {
            self.NewOrder().Sales.push(new SalesDetailVM({},self));
        }
    };

    //self.totalAmount = ko.computed(function () {
    //    var items = self.IsUpdated() ? self.SelectedOrder().Items() : self.NewOrder().Items();
    //    var total = items.reduce(function (total, item) {
    //        var price = parseFloat(item.Price()) || 0;
    //        var quantity = parseInt(item.Quantity()) || 0;
    //        return total + (price * quantity);
    //    }, 0);
    //    return total.toFixed(2);
    //});


    self.calculateTotal = function () {
        self.totalAmount.notifySubscribers();  // Recalculate the total amount
    };

    // Initialize the form
    self.resetForm();
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