using Gasware.Models;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasware.ViewModels
{
    public class MainWindowViewModel
    {
        public List<UserModel> mainUserModels { get; set; }

        public MainWindowViewModel()
        {
        }

        public MainWindowViewModel(IUserRespository userRespository)
        {
            mainUserModels = userRespository.GetUsers();
        }

    }
}
