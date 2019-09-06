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

        public Either<Error, Dollars> zeroDollars = new Left<Error, Dollars>(new Error() { Code = Code.Zero });

        #region map_adapter_1

        [Test]
        public void When_EitherMapAdapter1Called_Expect_RightValue()       
        {                                                               
            Either<Error,double> restMoney = rightDollars.Map(Pay10dollars);              
            Assert.That( restMoney is Right<Error, double> e && (double)e == 30);
        }

        [Test]
        public void When_EitherMapAdapter1Called_Expect_LeftValue()
        {
            Either<Error, double> error = leftDollars.Map(Pay10dollars);
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
            Either<Error, bool> isThatStatementTrue = rightDollars.Map(IWillBuyYouAPresent);
            Assert.That(isThatStatementTrue is Right<Error, bool> r && (bool)r == true);
            
        }
        
        [Test]
        public void When_EitherMapAdapter3Called_Expect_LeftValue()
        {
            Either<Error, bool> isThatStatementTrue = leftDollars.Map(IWillBuyYouAPresent);
            Assert.That(isThatStatementTrue is Left<Error, bool> e && ((Error)e).Code == Code.Minus);
        }

        #endregion

        #region reduce_adapter_1

        [Test]
        public void When_EitherReduceAdapter1Called_Expect_RightValue()
        {
            Either<Error, Dollars> balance = rightDollars.Reduce(HandleMinusError);
            Assert.That(balance is Right<Error, Dollars>);
        }

        [Test]
        public void When_EitherReduceAdapter1Called_Expect_LeftValue()
        {
            //leftDollars have minus
            Either<Error, Dollars> balance = leftDollars.Reduce(HandleMinusError);
            Assert.That(balance is Right<Error, Dollars> b && ((Dollars)b).Amount == -1);
        }

        #endregion

        #region reduce_adapter_2

        [Test]
        public void When_EitherReduceAdapter2Called_Expect_RightValueAndDollarMinus()
        {
            Either<Error, Dollars> balance = leftDollars.Reduce(HandleMinusError, e => e.Code is Code.Minus);
            Assert.That(balance is Right<Error, Dollars> b && ((Dollars)b).Amount == -1);
        }

        [Test]
        public void When_EitherReduceAdapter2Called_Expect_LeftValue()
        {
            Either<Error, Dollars> error = leftDollars.Reduce(HandleMinusError, e => e.Code is Code.Zero);
            Assert.That(error is Left<Error, Dollars> b);
        }

        [Test]
        public void When_EitherReduceAdapter2Called_Expect_RightValueAndPositiveDollars()
        {
            Either<Error, Dollars> error = rightDollars.Reduce(HandleMinusError, e => e.Code is Code.Minus);
            Assert.That(error is Right<Error, Dollars> b && ((Dollars)b).Amount > 0);
        }

        #endregion

        #region reduce_adapter_3

        [Test]
        public void When_EitherAdapretReduce3Called_Expect_LoanRightValue()
        {
            Either<Error, Dollars> load = zeroDollars.Reduce(Loan20Dollars);
            Assert.That(load is Right<Error, Dollars> l && ((Dollars)l).Amount == 20);
        }

        [Test]
        public void When_EitherAdapretReduce3Called_Expect_OriginalRightValue()
        {
            Either<Error, Dollars> load = rightDollars.Reduce(Loan20Dollars);
            Assert.That(load is Right<Error, Dollars> l && ((Dollars)l).Amount == 40);
        }

        #endregion


        #region test_helper_methods

        private Dollars Loan20Dollars()
        {
            return new Dollars() { Amount = 20 };
        }

        private Dollars HandleZeroError(Error error)
        {
            return new Dollars() { Amount = (int)error.Code };
            
        }

        private Dollars HandleMinusError(Error error)
        {
            return new Dollars() { Amount = (int)error.Code };
        }


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
        #endregion

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
        Zero = 0,
        Minus = -1
    }
}
