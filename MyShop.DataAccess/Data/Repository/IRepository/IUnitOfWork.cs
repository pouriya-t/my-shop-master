using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Data.Repository.IRepository
{
    public interface IUnitOfWork:IDisposable
    {

        ICategoryRepository Category { get;  }
        ISubCategoryRepository SubCategory { get; }
        IServiceRepository Service { get;  }
        IShoppingCartRepository ShoppingCart { get;  }
        IApplicationUserRepository ApplicationUser { get;  }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailsRepository OrderDetails { get; }
        IUserRepository User { get; }
        ICouponRepository Coupon { get; }
        void Save();
    }
}
