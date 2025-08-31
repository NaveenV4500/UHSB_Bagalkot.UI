using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;


namespace UHSB_Bagalkot.Service.Repositories
{
    
        public abstract class CommonConnection
        {
            protected readonly Uhsb2025Context _context;

            protected CommonConnection(Uhsb2025Context context)
            {
                _context = context;
            }
        }
    } 
