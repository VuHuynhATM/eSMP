using eSMP.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class ItemViewModel
    {
        public int ItemID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Discount { get; set; }
        public float Rate { get; set; }
        public double Price { get; set; }
        public string Item_Image { get; set; }
        public string Province { get; set; }
    }
    public class ItemModel
    {
        public int ItemID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Rate { get; set; }
        public double MaxPrice { get; set; }
        public double MinPrice { get; set; }
        public double Discount { get; set; }
        public DateTime Create_date { get; set; }
        public int Sub_CategoryID { get; set; }
        public Item_Status Item_Status { get; set; }
        public List<Image> List_Image { get; set; }
        public List<SpecificationTagModel> Specification_Tag { get; set; }
        public StoreViewModel Store { get; set; }
        public List<SubItemModel> ListSubItem { get; set; }
        public List<BrandModel> ListModel { get; set; }
        public double? Fax { get; set; }
    }
    public class SubItemModel
    {
        public int Sub_ItemID { get; set; }
        public string Sub_ItemName { get; set; }
        public int Amount { get; set; }
        public double Discount { get; set; }
        public double Price { get; set; }
        public int SubItem_StatusID { get; set; }
    }
    public class Sub_ItemRegister
    {
        public string Sub_ItemName { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
    }
    public class ItemImageRegister
    {
        public string FileName { get; set; }
        public string Path { get; set; }
    }
    public class ItemRegister
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Discount { get; set; }
        public int StoreID { get; set; }
        public int Sub_CategoryID { get; set; }
        public List<ItemImageRegister> List_Image { get; set; }
        public List<Sub_ItemRegister> List_SubItem { get; set; }
        public List<SpecificationTagRegister> List_Specitication { get; set; }
        public List<int> ListModel { get; set; }
    }
    public class SubItemUpdate
    {
        public int SubItemID { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
    }
}
