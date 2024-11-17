namespace InventoryManagementSystem.Models.VM
{
    public class PurchasesMasterVM
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string? VendorName { get; set; }
        public int InvoiceNumber { get; set; }
        public decimal BillAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; }
        public List<PurchasesDetailsVM> Purchases { get; set;}
    }

    public class PurchasesDetailsVM
    {
        public int Id { get; set; }
        public int ItemId {  get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }

    }
}

