using Gasware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasware.Repository.Interfaces
{
    public interface IUserRespository
    {
        UserModel Get(int id);

        UserModel GetUserByName(string username);

        List<UserModel> GetUsers();   
        void Update(UserModel userModel);

        int Create(UserModel user);


        void Delete(UserModel userModel);
    }
}
