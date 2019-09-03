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
            if(either is Right<TLeft, TRight>)
            {
                Right<TLeft, TRight> right = (Right<TLeft, TRight>)either;
                return (Either<TLeft, TRightResult>)func(right);
            }
            return (TLeft)(Left<TLeft,TRight>)either;
        }
        public static Either<TLeft, TRightResult> Map<TLeft, TRight, TRightResult>(this Either<TLeft,TRight> either, Func<TRight, Either<TLeft, TRightResult>> func)
        {
            if(either is Right<TLeft, TRight>)
            {
                Right<TLeft, TRight> right = (Right<TLeft, TRight>)either;
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
            else if (either is Left<TLeft, TRight>)
            {
                var x = (Left<TLeft, TRight>)either;
                return (TLeft)x;
            }else if (either == null)
            {
                return null;
            }else
            {
                var a = either.GetType();

                return null;
            }
        }

        public static TRight Reduce<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TLeft, TRight> func)
        {
            if(either is Left<TLeft, TRight>)
            {
                Left<TLeft, TRight> left = (Left<TLeft, TRight>)either;
                return func(left);
            }

            return (Right<TLeft, TRight>)either;
        }

        public static Either<TLeft, TRight> Reduce<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TLeft, TRight> func, Func<TLeft, bool> when)
        {
            if (either is Left<TLeft, TRight>)
            {
                Left<TLeft, TRight> left = (Left<TLeft, TRight>)either;

                return when(left) ?  func(left) : either;
            }

            return (Right<TLeft, TRight>)either;
        }

        //reduce either del without args
        public static TRight Reduce<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TRight> func)
        {
            if (either is Left<TLeft, TRight>)
            {
                Left<TLeft, TRight> left = (Left<TLeft, TRight>)either;
                return func();
            }

            return (Right<TLeft, TRight>)either;
        }
    }
}
