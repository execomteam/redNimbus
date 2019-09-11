using NUnit.Framework;
using RedNimbus.Either;
using System;
using System.Collections.Generic;
using System.Text;

namespace Either.Test
{
    [TestFixture]
    public class MapReduceCombinationTests
    {
        [Test]
        public void MapReduceCombination_OverSuccess1_ExpectOkOutcome()
        {
            //Arrange
            Either<IError, Success1> success1 = new Right<IError, Success1>(new Success1());
            //Act
            var result = success1
                .Map(s => s)                                            //map adapter 1
                .Map(ReturnSuccess)                                    //map adapter 2
                .Map(() => new Outcome("ok"))                           //map adapter 3
                .Reduce(ConvertError1ToOutcome, e => e is Error1)       //reduce adapter 2
                .Reduce(ConvertError2ToOutcome, e => e is Error2)       //reduce adapter 2
                .Reduce(ConvertDefaultErrorToOutcome);                  //reduce adapter 3
            //Assert
            Assert.That(result is Outcome);
            Assert.That(((Outcome)result).message == "ok");

        }

        [Test]
        public void MapReduceCombination_OverError1_ExpectError1Outcome()
        {
            //Arrange
            Either<IError, Success1> error1 = new Left<IError, Success1>(new Error1());
            //Act
            var result = error1
                .Map(s => s)                                            //map adapter 1
                .Map(ReturnSuccess)                                    //map adapter 2
                .Map(() => new Outcome("ok"))                           //map adapter 3
                .Reduce(ConvertError1ToOutcome, e => e is Error1)       //reduce adapter 2
                .Reduce(ConvertError2ToOutcome, e => e is Error2)       //reduce adapter 2
                .Reduce(ConvertDefaultErrorToOutcome);                  //reduce adapter 3
            //Assert
            Assert.That(result is Outcome);
            Assert.That(((Outcome)result).message == "error1");
        }

        [Test]
        public void MapReduceCombination_OverError2_ExpectError2Outcome()
        {
            //Arrange
            Either<IError, Success1> error2 = new Left<IError, Success1>(new Error2());
            //Act
            var result = error2
                .Map(s => s)                                            //map adapter 1
                .Map(ReturnSuccess)                                    //map adapter 2
                .Map(() => new Outcome("ok"))                           //map adapter 3
                .Reduce(ConvertError1ToOutcome, e => e is Error1)       //reduce adapter 2
                .Reduce(ConvertError2ToOutcome, e => e is Error2)       //reduce adapter 2
                .Reduce(ConvertDefaultErrorToOutcome);                  //reduce adapter 3
            //Assert
            Assert.That(result is Outcome);
            Assert.That(((Outcome)result).message == "error2");
        }

        [Test]
        public void MapReduceCombination_OverError3_ExpectDefaultErrorOutcome()
        {
            //Arrange
            Either<IError, Success1> error3 = new Left<IError, Success1>(new Error3());
            //Act
            var result = error3
                .Map(s => s)                                            //map adapter 1
                .Map(ReturnSuccess)                                    //map adapter 2
                .Map(() => new Outcome("ok"))                           //map adapter 3
                .Reduce(ConvertError1ToOutcome, e => e is Error1)       //reduce adapter 2
                .Reduce(ConvertError2ToOutcome, e => e is Error2)       //reduce adapter 2
                .Reduce(ConvertDefaultErrorToOutcome);                  //reduce adapter 3
            //Assert
            Assert.That(result is Outcome);
            Assert.That(((Outcome)result).message == "defaultError");
        }

        public Either<IError, Success2> ReturnSuccess(Success1 either)
        {
            return new Right<IError, Success2>(new Success2());
        }

        public Outcome ConvertError1ToOutcome(IError error)
        {
            return new Outcome("error1");
        }

        public Outcome ConvertError2ToOutcome(IError error)
        {
            return new Outcome("error2");
        }

        public Outcome ConvertDefaultErrorToOutcome()
        {
            return new Outcome("defaultError");
        }

    }

}
