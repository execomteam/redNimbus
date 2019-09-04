using NUnit.Framework;
using System;
using RedNimbus.Either;

namespace Either.Test
{
    [TestFixture]
    public class EitherTest
    {
        public Either<string, int> either = new Right<string, int>(0);

        [Test]
        public void When_EitherMapAdapter1Called()
        {

            Assert.That(
                (Right<string,int>)either
                    .Map(Increment) == 1);
        }

        public void When_EitherMapAdapter2Called()
        {
            either.Map(Increment);
        }

        private int Increment(int x)
        {
            return ++x;
        }


    }

    
}
