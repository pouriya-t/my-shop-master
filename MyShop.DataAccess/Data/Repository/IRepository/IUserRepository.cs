using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Data.Repository.IRepository
{
    public interface IUserRepository:IRepository<ApplicationUser>
    {
        void LockUser(string userId);
        void UnlockUser(string userId);
    }
}
