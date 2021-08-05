using System.Collections.Generic;

namespace VISABConnector.Example.Shooter
{
    public class ShooterImages : IImageContainer
    {
        public IDictionary<string, byte[]> PlayerAvatars { get; set; }

        public byte[] Map { get; set; }
    }
}