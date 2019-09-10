using NUnit.Framework;
using RedNimbus.Either;
using System;
using System.Collections.Generic;
using System.Text;

namespace Either.Test
{
    [TestFixture]
    public class MapAdapter2Tests
    {
        [Test]
        public void When_EitherMapAdapter2Called_Expect_Right()
        {
            //Arrange
            Either<IError, Success1> success = new Right<IError, Success1>(new Success1());
            //Act
            var result = success.Map(ReturnSuccess);
            //Assert
            Assert.That(result is Right<IError, Success2>);
        }

        [Test]
        public void When_EItherMapAdapter2Called_Expect_Left()
        {
            //Arrange
            Either<IError, Success1> error = new Left<IError, Success1>(new Error1());
            //Act
            var result = error.Map(ReturnSuccess);
            //Assert
            Assert.That(result is Left<IError, Success2>);
        }

        public Either<IError, Success2> ReturnSuccess(Success1 either)
        {
            return new Right<IError, Success2>(new Success2());
        }
    }

}
