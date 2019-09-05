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

        //Map adaptert 1
        [Test]
        public void When_EitherMap1AdapterCalled_Expect_RightValue()
        {
            double restMoney = (Right<Error, double>)rightDollars.Map(Pay10dollars);           
            Assert.That( restMoney == 30);
        }

        [Test]
        public void When_EitherMap1AdaptersCalled_Expect_LeftValue()
        {
            Error error = (Left<Error, double>)leftDollars.Map(Pay10dollars);
            Assert.That(error.Code == Code.Minus);

        }

        //Map adapter 2
        [Test]
        public void WhenEitherMap2AdapterCalled_Expect_RightValue()
        {
            Dollars exchanged = (Right<Error, Dollars>)rightEuros.Map(ExchangeEuroToDollars);
            Assert.That(exchanged.Amount == 80);
        }

        



        private double Pay10dollars(Dollars money)
        {
            return money.Amount -= 10;
        }

        private Either<Error, Dollars> ExchangeEuroToDollars(Euros euros)
        {
            return new Right<Error, Dollars>(new Dollars() { Amount = euros.Amount * 0.8 });
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
