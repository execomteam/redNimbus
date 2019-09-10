using NUnit.Framework;
using RedNimbus.Either;
using System;
using System.Collections.Generic;
using System.Text;

namespace Either.Test
{
    [TestFixture]
    public class ReduceAdapter3Tests
    {
        [Test]
        public void When_EitherAdapterReduce3Called_Expect_Outcome1()
        {
            //Assert
            Either<IError, Outcome> outcomeOk = new Right<IError, Outcome>(new Outcome("ok"));
            //Act
            var result = outcomeOk.Reduce(ConvertErrorToOutcome);
            //Assert
            Assert.That(result is Outcome);
            Assert.That(((Outcome)result).message == "ok");
        }

        [Test]
        public void When_EitherAdapterReduce3Called_Expect_Outcome2()
        {
            //Assert
            Either<IError, Outcome> outcomeErr1 = new Left<IError, Outcome>(new Error1());
            //Act
            var result = outcomeErr1.Reduce(ConvertErrorToOutcome);
            //Assert
            Assert.That(result is Outcome);
            Assert.That(((Outcome)result).message == "error");
        }

        public Outcome ConvertErrorToOutcome()
        {
            return new Outcome("error");
        }
    }

}
