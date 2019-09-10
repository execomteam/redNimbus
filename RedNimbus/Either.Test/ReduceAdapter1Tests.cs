using NUnit.Framework;
using RedNimbus.Either;
using System;
using System.Collections.Generic;
using System.Text;

namespace Either.Test
{
    [TestFixture]
    public class ReduceAdapter1Tests
    {
        [Test]
        public void When_EitherReduceAdapter1Called_Expect_Right()
        {
            //Arrange
            Either<IError, Outcome> outcomeOk = new Right<IError, Outcome>(new Outcome("ok"));
            //Act
            var res = outcomeOk.Reduce(ConvertErrorToOutcome);
            //Assert
            Assert.That(res is Outcome);
            Assert.That(res.message == "ok");
        }

        [Test]
        public void When_EitherReduceAdapter1Called_Expect_Left()
        {
            //Arrange
            Either<IError, Outcome> outcomeErr = new Left<IError, Outcome>(new Error1());
            //Act
            var res = outcomeErr.Reduce(ConvertErrorToOutcome);
            //Assert
            Assert.That(res is Outcome);
            Assert.That(res.message == "error");
        }

        public Outcome ConvertErrorToOutcome(IError error)
        {
            return new Outcome("error");
        }
    }

}
