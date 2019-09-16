using NUnit.Framework;
using System;
using RedNimbus.Either;

namespace Either.Test
{

    [TestFixture]
    public class MapAdapter1Tests
    {
        [Test]
        public void Map_OverSuccess1_ExpectSuccess1()
        {
            //Arrange
            Either<IError, Success1> success = new Right<IError, Success1>(new Success1());
            //Act
            var result = success.Map(NewReturnSuccess1);
            //Assert
            Assert.That(result is Right<IError, Success1>);
        }

        [Test]
        public void Map_OverError1_ExpectError1()
        {
            //Arrange
            Either<IError, Success1> error = new Left<IError, Success1>(new Error1());
            //Act
            var result = error.Map(NewReturnSuccess1);
            //Assert
            Assert.That(result is Left<IError, Success1>);
        }

        public Success1 NewReturnSuccess1()
        {
            return new Success1();
        }
    }
}