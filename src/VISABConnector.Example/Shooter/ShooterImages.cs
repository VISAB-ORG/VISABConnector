using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VISABConnector.Example.Shooter
{
    public class ShooterImages : IImageContainer
    {
        public IDictionary<string, byte[]> PlayerAvatars { get; set; }

        public byte[] Map { get; set; }
    }
}
