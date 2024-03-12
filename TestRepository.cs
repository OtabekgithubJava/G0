using Newtonsoft.Json;

namespace DTM
{
    public class TestRepository
    {
        public List<Testcss>? GetTests()
        {
            var stringTest = File.ReadAllText("/Users/otabek_coding/Desktop/Najot Ta'lim/DTM/Tests.json");

            var tests = JsonConvert.DeserializeObject<List<Testcss>>(stringTest);

            return tests;
        }
    }   
}