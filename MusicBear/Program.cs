using System;
using System.Threading.Tasks;
using MusicBear.Core;

namespace MusicBear
{
    public class Program
    {
        public static async Task Main(string[] args) => 
            await new EntryPoint().EntryAsync();
    }
}
