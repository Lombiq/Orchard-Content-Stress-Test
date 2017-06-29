using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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