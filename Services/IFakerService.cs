using Bogus;
using Orchard;

namespace Lombiq.OrchardContentStressTest.Services
{
    /// <summary>
    /// Service for Faker related data generation.
    /// </summary>
    public interface IFakerService : IDependency
    {
        /// <summary>
        /// Creates a Faker for generating data.
        /// </summary>
        Faker CreateFaker();
    }
}