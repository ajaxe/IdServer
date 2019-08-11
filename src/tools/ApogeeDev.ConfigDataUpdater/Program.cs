using System;
using Newtonsoft.Json;

namespace ApogeeDev.ConfigDataUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(JsonConvert.SerializeObject(new IdentityResourceConfigurator().Data(), Formatting.Indented));
            Console.WriteLine(JsonConvert.SerializeObject(new ClientConfigurator().Data(), Formatting.Indented));
            Console.WriteLine(JsonConvert.SerializeObject(new ApiResourceConfigurator().Data(), Formatting.Indented));

            Console.Read();
        }
    }
}