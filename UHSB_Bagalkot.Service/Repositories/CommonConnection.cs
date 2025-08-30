using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.DataAccess;

namespace UHSB_Bagalkot.Service.Repositories
{
    
        public abstract class CommonConnection
        {
            protected readonly AppDbContext _context;

            protected CommonConnection(AppDbContext context)
            {
                _context = context;
            }
        }
    } 
