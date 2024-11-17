/// <reference path="../knockout.js" />
/// <reference path="customer.model.js" />

const mode = {
    create: 1,
    update: 2
};

var CustomerController = function () {
    var self = this;
    const baseUrl = '/api/CustomerAPI';
    self.IsUpdated = ko.observable(false);
    self.NewCustomer = ko.observable(new CustomerModel());
    self.SelectedCustomer = ko.observableArray([]);
    self.CurrentCustomer = ko.observableArray([]);
    self.mode = ko.observable(mode.create);
    self.searchTerm = ko.observable('');
    self.searchArrayList = ko.observableArray([]);
    self.customerToDelete = ko.observable();
    self.GetDatas = function () {
        ajax.get(baseUrl).then(function (result) {
            self.CurrentCustomer(result.map(item => new CustomerModel(item))); 
        });
    }

    self.GetDatas();

    self.getSearchData = function () {
        if (self.searchTerm()) {
            console.log("Searching for:", self.searchTerm());
            ajax.get(baseUrl + "/Search?searchTerm=" + encodeURIComponent(self.searchTerm()))
                .then(function (result) {
                    console.log("Search results:", result);
                    self.searchArrayList(result.map(item => new CustomerModel(item)));
                    self.CurrentCustomer(self.searchArrayList());
                   // self.currentPage(1); // Reset to first page after search
                })
                .fail(function (error) {
                    console.error("Search failed:", error);
                });
        } else {
            console.log("No search term, getting all data");
            self.GetDatas();
        }
    }

    self.clickedSearch = function () {
        self.getSearchData();
    }

    self.validate = function () {
        var isvalid = true;
        var customer = self.IsUpdated() ? self.SelectedCustomer : self.NewCustomer;
        if (customer().FullName() == '' || customer().FullName().length > 25) {
            toastr.error("Please Enter your Name and must be less than 25");
            isvalid = false;
            return;
        }

        if (customer().ContactNo() == '') {
            toastr.error("Contact Number is Required");
            isvalid = false;
            return;
        }
        if (customer().ContactNo().length > 10 || customer().ContactNo().length < 10 || customer().ContactNo() <= 0) {
            toastr.error("Number must equal to 10 digit");
            isvalid = false;
            return;
        }
        if (customer().Address() == '') {
            toastr.error("Please Enter Your Address and must be less than 50 character");
            isvalid = false;
            return;
        }
        return isvalid;
    }

    self.AddCustomer = function () {
        var isvalid = self.validate();
        var customerData = ko.toJS(self.IsUpdated() ? self.SelectedCustomer : self.NewCustomer);
        if (isvalid) {
            switch (self.mode()) {
                case 1:
                    ajax.post(baseUrl, JSON.stringify(customerData))
                        .done(function (result) {
                            if (self.validate()) {
                                self.CurrentCustomer.push(new CustomerModel(result));
                                self.GetDatas();
                                self.CloseModel();
                                $('#customerModal').modal('hide');
                                Swal.fire("Customer Added Successfully");
                            }
                            else {
                                toastr.error("Error occur while adding Customer");
                            }
                        })
                    break;
                case 2:
                    ajax.put(baseUrl, JSON.stringify(customerData))
                        .done(function (result) {
                            self.CurrentCustomer.replace(self.SelectedCustomer(), new CustomerModel(result));
                            self.CloseModel();
                            self.GetDatas();
                            $('#customerModal').modal('hide');
                            toastr.success("Customer Updated Successfully");
                        })
            }
        }
    }

    self.DeleteCustomer = function (model) {
        self.customerToDelete(model);
        setTimeout(function () {
            $('#deleteConfirmModal').modal('show');
        }, 100);
    };

    self.confirmDelete = function ()
    {
        var model = self.customerToDelete();
        if (model) {
            ajax.delete(baseUrl + "?id=" + model.Id())
                .done((result) => {
                    self.CurrentCustomer.remove(model);
                    $('#deleteConfirmModal').modal('hide');
                    toastr.success("deleted Successfully")
                })
                .fail((err) => {
                    console.log(err);
                    $('#deleteConfirmModal').modal('hide');
                });
        }
    };

    self.SelectCustomer = function (model) {
        self.SelectedCustomer(model);
        self.IsUpdated(true);
        self.mode(mode.update);
        $('#customerModel').modal('show');
    }

    self.CloseModel = function () {
        self.ResetForm();
        self.GetDatas();
    }

    self.ResetForm = function () {
        self.NewCustomer(new CustomerModel());
        self.SelectedCustomer(new CustomerModel());
        self.IsUpdated(false);
        self.mode(mode.create);
    }
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
            url: route,
        });
    }
}