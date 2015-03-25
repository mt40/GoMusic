using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoMusic.Models
{
    public class SearchQuery
    {
        private string query;

        public string Query
        {
            get { return query; }
            set
            {
                if (value != "" && query != value)
                    query = value;
            }

        }

        public SearchQuery()
        {

        }

        public SearchQuery(string q)
        {
            query = q;
        }
    }
}
