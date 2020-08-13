﻿using Gasware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasware.ViewModels
{
    public class DeliveryPersonViewModel
    {
        public int DeliveryPersonId { get; set; }
        public string PhoneNumber { get; set; }
        public AddressModel Address { get; set; }
        public string Name { get; set; }
    }
}
