using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.DataAcess
{
    public class RepositoryFactory
    {
        public static IMySqlHelper BaseRepository(string name)
        {
            return new SqlHelper(name);
        }
    }
}
