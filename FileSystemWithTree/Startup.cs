using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FileSystemWithTree.Startup))]
namespace FileSystemWithTree
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
