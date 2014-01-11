using Forumz.Common.Crypto;
using Forumz.Common.Web;
using StructureMap;

namespace Forumz.Ifx.IoC
{
    public static class Bootstrapper
    {
        public static IContainer Init()
        {
            ObjectFactory.Configure(a =>
                                    {
                                        a.For<ISymmetricCrypto>()
                                         .Singleton()
                                         .Use<SymmetricCrypto>();

                                        a.For<IHttpContextProvider>()
                                         .Use<HttpContextProvider>();
                                    });

            return ObjectFactory.Container;
        }
    }
}