using System;

namespace Lykke.Frontend.WampHost.Documentation
{
    public class DocMeAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Type InputType { get; set; }
    }
}
