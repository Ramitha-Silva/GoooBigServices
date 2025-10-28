using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TpService.Models;
namespace TpService.Lib.BusinessLogic
{
    public class GetProductsIdsByUserId
    {
        private ShoppingCartDbConnection db;
        public GetProductsIdsByUserId(int userId)
        {
            db = new ShoppingCartDbConnection();
            var result = new List<long>();
            var InventoryId = db.ScInventories.Where(x => x.StorekeeperID == userId && x.IsActive == true && x.IsTrash == false).Select(x => x.AutoID).SingleOrDefault();
            var productsId = db.ScSubInventories.Where(x => x.InventoryID == InventoryId).Select(x => x.PackageID).ToList();


        }
        private bool isValidProduct(long productID)
        {
            return db.ScPackages.Where(x => x.AutoID == productID && x.IsActive == true && x.IsTrash == false).Any();
        }
    }
}