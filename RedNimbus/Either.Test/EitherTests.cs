using NUnit.Framework;
using System;
using RedNimbus.Either;

namespace Either.Test
{
    [TestFixture]
    public class EitherTest
    {
        Either<IError, Success1> success1 = new Right<IError, Success1>(new Success1());

        Either<IError, Success1> error1 = new Left<IError, Success1>(new Error1());
        Either<IError, Success1> error2 = new Left<IError, Success1>(new Error2());
        Either<IError, Success1> error3 = new Left<IError, Success1>(new Error3());

        Either<IError, Outcome> outcomeOk = new Right<IError, Outcome>(new Outcome("ok"));
        Either<IError, Outcome> outcomeErr1 = new Left<IError, Outcome>(new Error1());

        #region helpers

        public Either<IError, Success2> ReturnSuccess2(Success1 either)
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

        public Outcome ConvertDefaultErrorToOutcome2(IError error)
        {
            return new Outcome("defaultError");
        }

        #endregion

        #region map_adapter_1

        [Test]
        public void When_EitherMapAdapter1Called_Expect_Right()
        {

            var result = success1.Map((s) => new Success1());
            Assert.That(result is Right<IError, Success1>);
        }

        [Test]
        public void When_EitherMapAdapter1Called_Expect_Left()
        {
            var result = error1.Map((s) => new Success1());
            Assert.That(result is Left<IError, Success1>);
        }

        #endregion

        #region map_adapter_2

        [Test]
        public void When_EitherMapAdapter2Called_Expect_Right()
        {
            var result = success1.Map(ReturnSuccess2);
            Assert.That(result is Right<IError, Success2>);
        }


        [Test]
        public void When_EItherMapAdapter2Called_Expect_Left()
        {
            var result = error1.Map(ReturnSuccess2);
            Assert.That(result is Left<IError, Success2>);
        }

        #endregion

        #region map_adapter_3

        [Test]
        public void When_EitherMapAdapter3Called_Expect_Right()
        {
            var result = success1.Map(() => new Outcome("Ok"));
            Assert.That(result is Right<IError, Outcome>);
        }


        [Test]
        public void When_EitherMapAdapter3Called_Expect_Left()
        {
            var result = error1.Map(() => new Outcome("Ok"));
            Assert.That(result is Left<IError, Outcome>);
        }

        #endregion

        #region reduce_adapter_1

        [Test]
        public void When_EitherReduceAdapter1Called_Expect_Right()
        {
            Either<IError, Outcome> res = outcomeOk.Reduce(ConvertDefaultErrorToOutcome2);
            Assert.That(res is Right<IError, Outcome>);
        }

        [Test]
        public void When_EitherReduceAdapter1Called_Expect_Left()
        {
            Either<IError, Outcome> res = outcomeErr1.Reduce(ConvertDefaultErrorToOutcome2);
            Assert.That(res is Right<IError, Outcome>);
        }

        #endregion

        #region reduce_adapter_2

        [Test]
        public void When_EitherReduceAdapter2Called_Expect_Right1()
        {
            var result = outcomeOk.Reduce(ConvertError1ToOutcome, e => e is Error1);
            Assert.That(result is Right<IError, Outcome>);
        }

        [Test]
        public void When_EitherReduceAdapter2Called_Expect_Right2()
        {
            var result = outcomeErr1.Reduce(ConvertError1ToOutcome, e => e is Error1);
            Assert.That(result is Right<IError, Outcome>);
        }

        [Test]
        public void When_EitherReduceAdapter2Called_Expect_Left()
        {
            var result = outcomeErr1.Reduce(ConvertError1ToOutcome, e => e is Error2);
            Assert.That(result is Left<IError, Outcome>);
        }

        #endregion

        #region reduce_adapter_3

        [Test]
        public void When_EitherAdapterReduce3Called_Expect_Outcome1()
        {
            var result = outcomeOk.Reduce(ConvertDefaultErrorToOutcome);
            Assert.That(result is Outcome r && r.message == "ok");
        }


        [Test]
        public void When_EitherAdapterReduce3Called_Expect_Outcome2()
        {
            var result = outcomeErr1.Reduce(ConvertDefaultErrorToOutcome);
            Assert.That(result is Outcome r && r.message == "defaultError");
        }

        #endregion

        #region combinations

        [Test]
        public void When_CombineMapAndReduce_Expect_Ok()
        {
            var result = success1
                .Map(s => s)                                            //map adapter 1
                .Map(ReturnSuccess2)                                    //map adapter 2
                .Map(() => new Outcome("ok"))                           //map adapter 3
                .Reduce(ConvertError1ToOutcome, e => e is Error1)       //reduce adapter 2
                .Reduce(ConvertError2ToOutcome, e => e is Error2)       //reduce adapter 2
                .Reduce(ConvertDefaultErrorToOutcome);                  //reduce adapter 3

            Assert.That(result is Outcome r && r.message == "ok");

        }

        [Test]
        public void When_CombineMapAndReduce_Expect_Error1()
        {
            var result = error1
                .Map(s => s)                                            //map adapter 1
                .Map(ReturnSuccess2)                                    //map adapter 2
                .Map(() => new Outcome("ok"))                           //map adapter 3
                .Reduce(ConvertError1ToOutcome, e => e is Error1)       //reduce adapter 2
                .Reduce(ConvertError2ToOutcome, e => e is Error2)       //reduce adapter 2
                .Reduce(ConvertDefaultErrorToOutcome);                  //reduce adapter 3

            Assert.That(result is Outcome r && r.message == "error1");
        }

        [Test]
        public void When_CombineMapAndReduce_Expect_Error2()
        {
            var result = error2
                .Map(s => s)                                            //map adapter 1
                .Map(ReturnSuccess2)                                    //map adapter 2
                .Map(() => new Outcome("ok"))                           //map adapter 3
                .Reduce(ConvertError1ToOutcome, e => e is Error1)       //reduce adapter 2
                .Reduce(ConvertError2ToOutcome, e => e is Error2)       //reduce adapter 2
                .Reduce(ConvertDefaultErrorToOutcome);                  //reduce adapter 3

            Assert.That(result is Outcome r && r.message == "error2");
        }

        [Test]
        public void When_CombineMapAndReduce_Expect_DefaultError()
        {
            var result = error3
                .Map(s => s)                                            //map adapter 1
                .Map(ReturnSuccess2)                                    //map adapter 2
                .Map(() => new Outcome("ok"))                           //map adapter 3
                .Reduce(ConvertError1ToOutcome, e => e is Error1)       //reduce adapter 2
                .Reduce(ConvertError2ToOutcome, e => e is Error2)       //reduce adapter 2
                .Reduce(ConvertDefaultErrorToOutcome);                  //reduce adapter 3

            Assert.That(result is Outcome r && r.message == "defaultError");
        }

        #endregion

    }

    public class Outcome
    {
        public string message;

        public Outcome(string message)
        {
            this.message = message;
        }
    }

    public class Success1 { }

    public class Success2 { }

    public interface IError { }

    public class Error1 : IError { }

    public class Error2 : IError { }

    public class Error3 : IError { }
}
