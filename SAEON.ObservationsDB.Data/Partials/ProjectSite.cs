using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.ObservationsDB.Data
{
    public partial class ProjectSite
    {
        public string OrganisationName
        {
            get
            {
                String result = String.Empty;
                Organisation org = this.Organisation;

                if (org != null)
                    result = String.Concat(org.Code, " - ", org.Name);

                return result;
            }
        }
    }
}
