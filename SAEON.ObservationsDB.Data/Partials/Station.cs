using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.ObservationsDB.Data
{
    public partial class Station
    {

        public string ProjectSiteName
        {
            get
            {
                String result = String.Empty;
                ProjectSite ps = this.ProjectSite;

                if (ps != null)
                    result = String.Concat(ps.Code, " - ", ps.Name);

                return result;
            }
        }

        public string SiteName
        {
            get
            {
                String result = String.Empty;
                Site s = this.Site;

                if (s != null)
                    result = String.Concat(s.Code, " - ", s.Name);

                return result;
            }
        }
    }
}
