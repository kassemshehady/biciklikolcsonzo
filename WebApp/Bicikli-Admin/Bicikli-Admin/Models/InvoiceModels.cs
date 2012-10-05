using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bicikli_Admin.CommonClasses
{
    public class InvoiceModel
    {
        public string lender_name;
        public string lender_address;
        public string bike_name;
        public string name;
        public string address;
        public IEnumerable<InvoiceItemModel> items;
        public int total_balance;
    }

    public class InvoiceItemModel
    {
        public string title;
        public int amount;
        public int base_unit_price;
        public float vat;
    }
}