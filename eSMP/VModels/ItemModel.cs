using eSMP.Models;
using System.Collections.ObjectModel;
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
        public int Num_Sold { get; set; }
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
        public int Num_Sold { get; set; }
        public DateTime Create_date { get; set; }
        public int Sub_CategoryID { get; set; }
        public Status Item_Status { get; set; }
        public List<Image> List_Image { get; set; }
        public List<SpecificationTagModel> Specification_Tag { get; set; }
        public StoreViewModel Store { get; set; }
        public List<SubItemModel> ListSubItem { get; set; }
        public List<BrandModel> ListModel { get; set; }
        public List<FeedBackModel> ListFeedBack { get; set; }
    }
    public class ItemRegister_Sub
    {
        public string sub_ItemName { get; set; }
        public int amount { get; set; }
        public double price { get; set; }
    }
    public class SubItemModel
    {
        public int Sub_ItemID { get; set; }
        public string Sub_ItemName { get; set; }
        public int Amount { get; set; }
        public Image Image { get; set; }
        public double Price { get; set; }
        public Status SubItem_Status { get; set; }
    }
    public class Sub_ItemRegister
    {
        public int itemID { get; set; }
        public string Sub_ItemName { get; set; }
        public int Amount { get; set; }
        public IFormFile File { get; set; }
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
        public IFormFileCollection List_Image { get; set; }
        public IFormFileCollection List_SubItem_Image { get; set; }
        public string List_SubItem { get; set; }
        public string List_Specitication { get; set; }
        public string ListModel { get; set; }
    }
    public class SubItemUpdate
    {
        public int SubItemID { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
    }
}
