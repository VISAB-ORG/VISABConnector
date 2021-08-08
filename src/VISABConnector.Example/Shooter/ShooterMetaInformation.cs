using System.Collections.Generic;

namespace VISABConnector.Example.Shooter
{
    public class ShooterMetaInformation : IMetaInformation
    {
        public string Game => "MeShooter";

        public IList<string> PlayerNames { get; set; }
    }
}