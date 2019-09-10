using NUnit.Framework;
using RedNimbus.Either;
using System;
using System.Collections.Generic;
using System.Text;

namespace Either.Test
{
    [TestFixture]
    public class ReduceAdapter2Tests
    {
        [Test]
        public void When_EitherReduceAdapter2Called_Expect_Right1()
        {
            //Arrange
            Either<IError, Outcome> outcomeOk = new Right<IError, Outcome>(new Outcome("ok"));
            //Act
            var result = outcomeOk.Reduce(ConvertError1ToOutcome, e => e is Error1);
            //Assert
            Assert.That(result is Right<IError, Outcome>);
        }

        [Test]
        public void When_EitherReduceAdapter2Called_Expect_Right2()
        {
            //Arrange
            Either<IError, Outcome> outcomeErr = new Left<IError, Outcome>(new Error1());
            //Act
            var result = outcomeErr.Reduce(ConvertError1ToOutcome, e => e is Error1);
            //Assert
            Assert.That(result is Right<IError, Outcome>);
        }

        [Test]
        public void When_EitherReduceAdapter2Called_Expect_Left()
        {
            //Arrange
            Either<IError, Outcome> outcomeErr = new Left<IError, Outcome>(new Error1());
            //Act
            var result = outcomeErr.Reduce(ConvertError1ToOutcome, e => e is Error2);
            //Assert
            Assert.That(result is Left<IError, Outcome>);
        }
        public Outcome ConvertError1ToOutcome(IError error)
        {
            return new Outcome("error1");
        }
    }

}
