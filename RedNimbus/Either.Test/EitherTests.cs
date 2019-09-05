using NUnit.Framework;
using System;
using RedNimbus.Either;

namespace Either.Test
{
    [TestFixture]
    public class EitherTest
    {
        public Either<Error, Dollars> rightDollars = new Right<Error, Dollars>(new Dollars { Amount = 40 });
        public Either<Error, Euros> rightEuros = new Right<Error, Euros>(new Euros { Amount = 100 });

        public Either<Error, Dollars> leftDollars = new Left<Error, Dollars>(new Error() { Code = Code.Minus });
        public Either<Error, Euros> LeftEuros = new Left<Error, Euros>(new Error() { Code = Code.Minus });

        #region map_adapter_1

        [Test]
        public void When_EitherMapAdapter1Called_Expect_RightValue()
        {
            var restMoney = rightDollars.Map(Pay10dollars);           
            Assert.That( restMoney is Right<Error, double>);
        }

        [Test]
        public void When_EitherMapAdapter1Called_Expect_LeftValue()
        {
            var error = leftDollars.Map(Pay10dollars);
            Assert.That(error is Left<Error, double> e && ((Error)e).Code == Code.Minus);
        }

        #endregion


        #region map_adapter_2

        [Test]
        public void When_EitherMapAdapter2Called_Expect_RightValue()
        {
            Either<Error, Dollars> exchanged = rightEuros.Map(ExchangeEuroToDollars);
            Assert.That(exchanged is Right<Error, Dollars> e && ((Dollars)e).Amount == 80);
        }

        [Test]
        public void When_EItherMapAdapter2Called_Expect_LeftValue()
        {
            Either<Error, Dollars> error = LeftEuros.Map(ExchangeEuroToDollars);
            Assert.That(error is Left<Error, Dollars> e && ((Error)e).Code == Code.Minus);
        }

        #endregion

        #region map_adapter_3
        
        [Test]
        public void When_EitherMapAdapter3Called_Expect_RightValue()
        {
            bool isThatStatementTrue = (Right<Error,bool>)rightDollars.Map(IWillBuyYouAPresent);
            Assert.That(isThatStatementTrue);
            
        }
        
        [Test]
        public void When_EitherMapAdapter3Called_Expect_LeftValue()
        {
            Error isThatStatementTrue = (Left<Error, bool>)leftDollars.Map(IWillBuyYouAPresent);
            Assert.That(isThatStatementTrue.Code == Code.Minus);
        }

        #endregion

        private double Pay10dollars(Dollars money)
        {
            return money.Amount -= 10;
        }

        private Either<Error, Dollars> ExchangeEuroToDollars(Euros euros)
        {
            return new Right<Error, Dollars>(new Dollars() { Amount = euros.Amount * 0.8 });
        }

        private bool IWillBuyYouAPresent()
        {
            return true;
        }

    }

    public class Dollars
    {
        public double Amount { get; set; }
    }

    public class Euros
    {
        public double Amount { get; set; }
    }

    public class Error
    {
        public Code Code { get; set; }
    }

    public enum Code
    {
        OnZero = 0,
        Minus = 1
    }
}
