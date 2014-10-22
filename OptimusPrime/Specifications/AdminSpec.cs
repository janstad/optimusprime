using System.Configuration;
using System.Linq;

namespace OptimusPrime.Specifications
{
    public class AdminSpec
    {
        public bool IsSatisfiedBy(string pUserName)
        {
            var vAdminArray = ConfigurationManager.AppSettings["SystemAdmins"].Split(',');
            return vAdminArray.Contains(pUserName);
        }
    }
}
