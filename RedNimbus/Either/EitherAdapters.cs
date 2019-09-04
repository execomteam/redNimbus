using RedNimbus.Either;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.Either
{
    public static class EitherAdapters
    {
        
        public static Either<TLeft, TRightResult> Map<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either, Func<TRight, TRightResult> func)
        {
            if (either is Right<TLeft, TRight> right)
            {
                return func(right);
            }
            return (TLeft)(Left<TLeft,TRight>)either;
        }
        public static Either<TLeft, TRightResult> Map<TLeft, TRight, TRightResult>(this Either<TLeft,TRight> either, Func<TRight, Either<TLeft, TRightResult>> func)
        {
            if (either is Right<TLeft, TRight> right)
            {
                return func(right);
            }
            return (TLeft)(Left<TLeft, TRight>)either;
        }

        public static Either<TLeft, TRightResult> Map<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either, Func<TRightResult> func)
        {
            if(either is Right<TLeft, TRight>)
            {
                return func();
            }
            else
            {
                var x = (Left<TLeft, TRight>)either;
                return (TLeft)x;
            }
        }

        public static TRight Reduce<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TLeft, TRight> func)
        {
            if (either is Left<TLeft, TRight> left)
            {
                return func(left);
            }

            return (Right<TLeft, TRight>)either;
        }

        public static Either<TLeft, TRight> Reduce<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TLeft, TRight> func, Func<TLeft, bool> when)
        {
            if (either is Left<TLeft, TRight> left)
            {
                return when(left) ?  func(left) : either;
            }

            return (Right<TLeft, TRight>)either;
        }

        public static TRight Reduce<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TRight> func)
        {
            if (either is Left<TLeft, TRight>)
            {
                return func();
            }

            return (Right<TLeft, TRight>)either;
        }
    }
}
