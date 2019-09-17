using RedNimbus.Either;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.Either
{
    public static class EitherAdapters
    {
        
        /// <summary>
        /// 1
        /// </summary>
        public static Either<TLeft, TRightResult> Map<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either, Func<TRight, TRightResult> func)
        {
            if (either is Right<TLeft, TRight> right)
            {
                return func(right);
            }
            return (TLeft)(Left<TLeft,TRight>)either;
        }

        /// <summary>
        /// 2
        /// </summary>
        public static Either<TLeft, TRightResult> Map<TLeft, TRight, TRightResult>(this Either<TLeft,TRight> either, Func<TRight, Either<TLeft, TRightResult>> func)
        {
            if (either is Right<TLeft, TRight> right)
            {
                return func(right);
            }
            return (TLeft)(Left<TLeft, TRight>)either;
        }

        /// <summary>
        /// 3
        /// </summary>
        public static Either<TLeft, TRightResult> Map<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either, Func<TRightResult> func)
        {
            if(either is Right<TLeft, TRight>)
            {
                return func();
            }

            return (TLeft)(Left<TLeft, TRight>)either;
        }

        /// <summary>
        /// 4
        /// </summary>
        public static Either<TLeft, TRightResult> Map<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either, Func<TRightResult> func, Action<TRight> logAction)
        {
            if (either is Right<TLeft, TRight> right)
            {
                logAction(right);
                return func();
            }

            return (TLeft)(Left<TLeft, TRight>)either;
        }

        /// <summary>
        /// 5
        /// </summary>
        public static Either<TLeft, TRightResult> Map<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either, Func<TRight, TRightResult> func, Action<TRight> action)
        {
            if (either is Right<TLeft, TRight> right)
            {
                action(right);
                return func(right);
            }
            return (TLeft)(Left<TLeft, TRight>)either;
        }


        /// <summary>
        /// 1
        /// </summary>
        public static TRight Reduce<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TLeft, TRight> func)
        {
            if (either is Left<TLeft, TRight> left)
            {
                return func(left);
            }

            return (Right<TLeft, TRight>)either;
        }


        /// <summary>
        /// 2
        /// </summary>
        public static Either<TLeft, TRight> Reduce<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TLeft, TRight> func, Func<TLeft, bool> when)
        {
            if (either is Left<TLeft, TRight> left)
            {
                return when(left) ? func(left) : either;
            }

            return (Right<TLeft, TRight>)either;
        }


        /// <summary>
        /// 3
        /// </summary>
        public static TRight Reduce<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TRight> func)
        {
            if (either is Left<TLeft, TRight>)
            {
                return func();
            }

            return (Right<TLeft, TRight>)either;
        }

        /// <summary>
        /// 4
        /// </summary>
        public static Either<TLeft, TRight> Reduce<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TLeft, TRight> func, Func<TLeft, bool> when, Action<TLeft> logAction)
        {
            if (either is Left<TLeft, TRight> left)
            {
                if (when(left))
                {
                    logAction(left);
                    return func(left); 
                }

                return either;
            }

            return either;
        }

        /// <summary>
        /// 5
        /// </summary>
        public static TRight Reduce<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TLeft, TRight> func, Action<TLeft> logAction)
        {
            if (either is Left<TLeft, TRight> left)
            {
                logAction(left);
                return func(left);
            }

            return (Right<TLeft, TRight>)either;
        }
    }
}
