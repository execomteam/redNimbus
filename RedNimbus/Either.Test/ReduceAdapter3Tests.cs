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
        public void Reduce_OverOkOutcome_ExpectOkOutcome()
        {
            //Assert
            Either<IError, Outcome> outcomeOk = new Right<IError, Outcome>(new Outcome("ok"));
            //Act
            var result = outcomeOk.Reduce(ConvertError1ToOutcome);
            //Assert
            Assert.That(result is Outcome);
            Assert.That(((Outcome)result).message == "ok");
        }

        [Test]
        public void Reduce_OverError1_ExpectError1Outcome()
        {
            //Assert
            Either<IError, Outcome> outcomeErr1 = new Left<IError, Outcome>(new Error1());
            //Act
            var result = outcomeErr1.Reduce(ConvertError1ToOutcome);
            //Assert
            Assert.That(result is Outcome);
            Assert.That(((Outcome)result).message == "error1");
        }

        public Outcome ConvertError1ToOutcome()
        {
            return new Outcome("error1");
        }
    }

}
