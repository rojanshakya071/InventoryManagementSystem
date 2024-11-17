/// <reference path="../knockout.js" />

const mode = {
    create: 1,
    update: 2
};

var ItemController = function () {
    var self = this;
    var valid;
    const baseUrl = '/api/ItemAPI';
    self.NewItem = ko.observable(new ItemModel());
    self.CurrentItem = ko.observableArray([]);
    self.SelectedItem = ko.observable(new ItemModel());
    self.IsUpdated = ko.observable(false);
    self.mode = ko.observable(mode.create);
    self.currentPage = ko.observable(1);
    self.itemsPerPage = ko.observable(10);
    self.itemToDelete = ko.observable();
    self.totalPages = ko.computed(function () {
        return Math.ceil(self.CurrentItem().length / self.itemsPerPage());
    });

    self.pagedItems = ko.computed(function () {
        var startIndex = (self.currentPage() - 1) * self.itemsPerPage();
        return self.CurrentItem.slice(startIndex, startIndex + self.itemsPerPage());
    });

    // Navigation methods
    self.nextPage = function () {
        if (self.currentPage() < self.totalPages()) {
            self.currentPage(self.currentPage() + 1);
        }
    };

    self.previousPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
        }
    };

    self.goToPage = function (page) {
        self.currentPage(page);
    };

    self.GetDatas = function () {
        ajax.get(baseUrl).then(function (result) {
            self.CurrentItem(result.map(item => new ItemModel(item)));
            self.currentPage(1);
        });
    };

    self.GetDatas();

    self.validate = function () {
        var isvalid = true;
        var item = self.IsUpdated() ? self.SelectedItem : self.NewItem;
        if (item().Name() == '' || item().Name().length > 25) {
            toastr.error("Please Enter your Name and must be less than 25");
            isvalid = false;
            return;
        }

        if (item().Unit() == '') {
            toastr.error("Please Enter Unit");
            isvalid = false;
            return;
        }
        if (item().Category() == '') {
            toastr.error("Please Enter Category");
            isvalid = false;
            return;
        }
        return isvalid;
    }

    self.AddItem = function () {

        var isvalid = self.validate();

        var ItemData = ko.toJS(self.IsUpdated() ? self.SelectedItem : self.NewItem);
        if (isvalid) {
            switch (self.mode()) {
                case mode.create:
                    ajax.post(baseUrl, JSON.stringify(ItemData))
                        .done(function (result) {
                            if (result.success) {
                                self.CurrentItem.push(new ItemModel(result));
                                self.CloseModel();
                                self.GetDatas();
                                $('#itemModal').modal('hide');
                                Swal.fire("Item Added Successfully");
                            } else {
                                toastr.error(result.message);
                            }

                        });
                    break;
                case mode.update:
                    ajax.put(baseUrl, JSON.stringify(ItemData))
                        .done(function (result) {
                            if (result.success) {
                                self.CurrentItem.replace(self.SelectedItem(), new ItemModel(result));
                                self.CloseModel();
                                self.GetDatas();
                                $('#itemModal').modal('hide');
                                toastr.success("Item Update Successfully");
                            }
                            else {
                                toastr.error(result.message);
                            }

                        });
                    break;
            }
        }

    };

    self.DeleteItem = function (model) {
        self.itemToDelete(model);
        setTimeout(function () {
            $('#deleteConfirmModal').modal('show');
        }, 100);
    };

    self.confirmDelete = function () {
        var model = self.itemToDelete();
        if (model) {
            ajax.delete(baseUrl + "?id=" + model.Id())
                .done((result) => {
                    self.CurrentItem.remove(model);
                    $('#deleteConfirmModal').modal('hide');
                    toastr.success("deleted Successfully")
                })
                .fail((err) => {
                    console.log(err);
                    $('#deleteConfirmModal').modal('hide');
                });
        }
    };

    self.SelectItem = function (model) {
        self.SelectedItem(model);
        self.IsUpdated(true);
        self.mode(mode.update);
    };

    self.CloseModel = function () {
        self.ResetForm();
        $('#itemModal').modal('hide');
    };

    self.ResetForm = function () {
        self.NewItem(new ItemModel());
        self.SelectedItem(new ItemModel());
        self.IsUpdated(false);
        self.mode(mode.create);
    };
};

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
