using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Gooobig
{

    public class FullProductInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string Code { get; set; }
        public string BarCode { get; set; }
        public string ImageUrl { get; set; }
        public List<string> ImagesUrls { get; set; }

        public string GroupCode { get; set; }

        public decimal Qnt { get; set; }
        public decimal Price { get; set; }

      //  public decimal? Cost { get; set; }
        public decimal? Tax { get; set; }
      //  public decimal? Discount { get; set; }

         public Measure MeasureType { get; set; }
        public Section Section { get; set; }

        
        public string Description { get; set; }

        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public List<ProductPrices> Prices { get; set; }

        //public List<AttributeViewModel> Attributes { get; set; }

        // public List<KeyValuePair<string,IEnumerable<string>>> Attributes { get; set; }
    }


    public class MiniProductInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }
        public string BarCode { get; set; }
        public string ImageUrl { get; set; }

        public string GroupCode { get; set; }

        public decimal Qnt { get; set; }
        public decimal Price { get; set; }

        public decimal? Cost { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Discount { get; set; }

        public int SectionId { get; set; }

        public ProductPrices Prices { get; set; }
        public string Description { get; set; }

    }

    public class AttributeViewModel
    {
        public string Parent { get; set; }
        public List<string> Childs { get; set; }

        public string FakeParent { get; set; }
        public string FakeChild { get; set; }
    }
    public class ProductPrices
    {
        public string Code { get; set; }
        public decimal Price { get; set; }
    }
    public class Measure
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

    }
    public class Section
    {
        public int Id { get; set; }
        public string Name { get; set; }
 
    }

    public class BackageBundleSubInformation
    {
        public long MainID { get; set; }
        public long PackageID { get; set; }
        public string Note { get; set; }
        public double? Qnt { get; set; }
    }
}