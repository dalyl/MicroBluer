
namespace LazyWelfare.AndroidMobile.Models
{
    using LazyWelfare.Interface;

    public class Argument : IArgument
    {
        public string Name { get; set; }

        public string Service { get; set; }

        public string Uri{ get; set; }
}
}