﻿using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services
{
    public class SpecificationRepository:ISpecificationReposity
    {
        private readonly WebContext _context;

        public SpecificationRepository(WebContext context)
        {
            _context = context;
        }
        public Boolean SpecificationIsExist(string name)
        {
            var specification = _context.Specification.SingleOrDefault(s => s.SpecificationName == name);
            if (specification == null)
                return false;
            return true;
        }
        public Specification GetSpecification(int spectificationID)
        {
            var specification=_context.Specification.SingleOrDefault(s => s.SpecificationID == spectificationID);
            if (specification == null)
                return null;
            return specification;
        }
        public Result GetAllSpecification()
        {
            Result result = new Result();
            try
            {
                var list=_context.SubCate_Specifications.ToList();
                List<Specification> speList=new List<Specification>();
                if(list.Count > 0)
                {
                    foreach(var spec in list)
                    {
                        Specification specification=GetSpecification(spec.Sub_CategoryID);
                        speList.Add(specification);
                    }
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = speList;
                    return result;
                }
                result.Success = true;
                result.Message = "Chưa có thông số";
                result.Data = "";
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
        public Specification GetSpecificationByID(int specificationID)
        {
            try
            {
                var spec = _context.Specification.SingleOrDefault(s => s.SpecificationID == specificationID);
                if (spec != null)
                {
                    return spec;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public Result GetSpecificationsBySubCate(int subCategoryID)
        {
            Result result = new Result();
            try
            {
                var listSpe = _context.SubCate_Specifications.Where(c => c.Sub_CategoryID == subCategoryID).ToList();
                List<Specification> specifications = new List<Specification>();
                if (listSpe.Count > 0)
                {
                    foreach (var item in listSpe)
                    {
                        var spe = GetSpecificationByID(item.SpecificationID);
                        specifications.Add(spe);
                    }
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = specifications;
                    return result;
                }
                result.Success = false;
                result.Message = "Chưa có thông số kĩ thuật";
                result.Data = new List<Specification>();
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi Hệ thống";
                result.Data = "";
                return result;
            }
        }
        public List<SpecificationTagModel> GetSpecificationsForItem(int itemID)
        {
            try
            {
                var listSpe=_context.Specification_Values.Where(c => c.ItemID == itemID).ToList();
                List<SpecificationTagModel> result = new List<SpecificationTagModel>();
                if(listSpe.Count > 0)
                {
                    foreach(var item in listSpe)
                    {
                        SpecificationTagModel tag = new SpecificationTagModel
                        {
                            ItemID = item.ItemID,
                            IsActive = item.IsActive,
                            SpecificationID = item.SpecificationID,
                            Specification_ValueID = item.Specification_ValueID,
                            Value = item.Value,
                            SpecificationName = GetSpecification(item.SpecificationID).SpecificationName,
                        };
                        result.Add(tag);
                    }
                    return result;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
    }
}