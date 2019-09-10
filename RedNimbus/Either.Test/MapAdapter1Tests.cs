using NUnit.Framework;
using System;
using RedNimbus.Either;

namespace Either.Test
{

    [TestFixture]
    public class MapAdapter1Tests
    {
        [Test]
        public void When_EitherMapAdapter1Called_Expect_Right()
        {
            //Arrange
            Either<IError, Success1> success = new Right<IError, Success1>(new Success1());
            //Act
            var result = success.Map((s) => new Success1());
            //Assert
            Assert.That(result is Right<IError, Success1>);
        }

        [Test]
        public void When_EitherMapAdapter1Called_Expect_Left()
        {
            //Arrange
            Either<IError, Success1> error = new Left<IError, Success1>(new Error1());
            //Act
            var result = error.Map((s) => new Success1());
            //Assert
            Assert.That(result is Left<IError, Success1>);
        }
    }
}
