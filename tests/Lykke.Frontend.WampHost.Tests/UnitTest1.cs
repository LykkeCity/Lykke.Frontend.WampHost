using Xunit;

namespace Lykke.Frontend.WampHost.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var host = new WampSharp.V2.WampHost();
            var realm = host.RealmContainer.GetRealmByName("main");

            var subject = realm.Services.GetSubject<string>("topic");

            subject.OnNext("aaa");
        }
    }

}
