using Bogus;

namespace Lombiq.OrchardContentStressTest.Services
{
    public class FakerService : IFakerService
    {
        public Faker CreateFaker()
        {
            return new Faker();
        }
    }
}