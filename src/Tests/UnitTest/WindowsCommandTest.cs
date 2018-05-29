using LazyWelfare.ServerHost.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class WindowsCommandTest
    {
        [TestMethod]
        public void VolumeTest()
        {
            var service =new WindowsVolume();

            var volume = new decimal(0.10) ;
            
            service.SetValue(volume);

            var value = service.GetValue();

            Assert.AreEqual(volume, value);

            service.SetValue(0);

            value = service.GetValue();

            Assert.AreEqual(0, value);

        }
    }
}
