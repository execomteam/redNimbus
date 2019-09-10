using NUnit.Framework;
using RedNimbus.Either;
using System;
using System.Collections.Generic;
using System.Text;

namespace Either.Test
{
    [TestFixture]
    public class MapAdapter3Tests
    {
        [Test]
        public void When_EitherMapAdapter3Called_Expect_Right()
        {
            //Arrange
            Either<IError, Success1> success = new Right<IError, Success1>(new Success1());
            //Act
            var result = success.Map(() => new Outcome("Ok"));
            //Assert
            Assert.That(result is Right<IError, Outcome>);
        }


        [Test]
        public void When_EitherMapAdapter3Called_Expect_Left()
        {
            //Arrange
            Either<IError, Success1> error = new Left<IError, Success1>(new Error1());
            //Act
            var result = error.Map(() => new Outcome("Ok"));
            //Assert
            Assert.That(result is Left<IError, Outcome>);
        }
    }

}
