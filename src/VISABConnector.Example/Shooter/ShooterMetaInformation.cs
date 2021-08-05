using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VISABConnector.Example.Shooter
{
    public class ShooterMetaInformation : IMetaInformation
    {
        public string Game => "MeShooter";

        public IList<string> PlayerNames { get; set; }
    }
}
