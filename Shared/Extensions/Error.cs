using System;
using System.Diagnostics;
using System.Globalization;

namespace Shared.Extensions
{
    public static class Error
    {
        [DebuggerStepThrough]
        public static Exception Application(string message, params object[] args)
        {
            //return new ApplicationException(message.FormatCurrent(args));
            return new ApplicationException(message.FormatWith(args));
        }

        [DebuggerStepThrough]
        public static Exception Application(Exception innerException, string message, params object[] args)
        {
            //return new ApplicationException(message.FormatCurrent(args), innerException);
            return new ApplicationException(message.FormatWith(args), innerException);
        }

        [DebuggerStepThrough]
        public static Exception ArgumentNullOrEmpty(Func<string> arg)
        {
            var argName = arg.Method.Name;
            return new ArgumentException("String parameter '{0}' cannot be null or all whitespace.", argName);
        }

        [DebuggerStepThrough]
        public static Exception ArgumentNull(string argName)
        {
            return new ArgumentNullException(argName);
        }

        [DebuggerStepThrough]
        public static Exception ArgumentOutOfRange<T>(Func<T> arg)
        {
            var argName = arg.Method.Name;
            return new ArgumentOutOfRangeException(argName);
        }

        [DebuggerStepThrough]
        public static Exception ArgumentOutOfRange(string argName)
        {
            return new ArgumentOutOfRangeException(argName);
        }

        [DebuggerStepThrough]
        public static Exception ArgumentOutOfRange(string argName, string message, params object[] args)
        {
            return new ArgumentOutOfRangeException(argName, string.Format(CultureInfo.CurrentCulture, message, args));
        }

        [DebuggerStepThrough]
        public static Exception Argument(string argName, string message, params object[] args)
        {
            return new ArgumentException(string.Format(CultureInfo.CurrentCulture, message, args), argName);
        }

        [DebuggerStepThrough]
        public static Exception Argument<T>(Func<T> arg, string message, params object[] args)
        {
            var argName = arg.Method.Name;
            //return new ArgumentException(message.FormatCurrent(args), argName);
            return new ArgumentException(message.FormatWith(args), argName);
        }

        [DebuggerStepThrough]
        public static Exception InvalidOperation(string message, params object[] args)
        {
            return InvalidOperation(message, null, args);
        }

        [DebuggerStepThrough]
        public static Exception InvalidOperation(string message, Exception innerException, params object[] args)
        {
            //return new InvalidOperationException(message.FormatCurrent(args), innerException);
            return new InvalidOperationException(message.FormatWith(args), innerException);
        }

        [DebuggerStepThrough]
        public static Exception InvalidOperation<T>(string message, Func<T> member)
        {
            return InvalidOperation(message, null, member);
        }

        [DebuggerStepThrough]
        public static Exception InvalidOperation<T>(string message, Exception innerException, Func<T> member)
        {
            Guard.NotNull(message, "message");
            Guard.NotNull(member, "member");

            //return new InvalidOperationException(message.FormatCurrent(member.Method.Name), innerException);
            return new InvalidOperationException(message.FormatWith(member.Method.Name), innerException);
        }

        [DebuggerStepThrough]
        public static Exception InvalidCast(Type fromType, Type toType)
        {
            return InvalidCast(fromType, toType, null);
        }

        [DebuggerStepThrough]
        public static Exception InvalidCast(Type fromType, Type toType, Exception innerException)
        {
            //return new InvalidCastException("Cannot convert from type '{0}' to '{1}'.".FormatCurrent(fromType.FullName, toType.FullName), innerException);
            return new InvalidCastException(
                "Cannot convert from type '{0}' to '{1}'.".FormatWith(fromType.FullName, toType.FullName),
                innerException);
        }

        [DebuggerStepThrough]
        public static Exception NotSupported()
        {
            return new NotSupportedException();
        }

        [DebuggerStepThrough]
        public static Exception NotImplemented()
        {
            return new NotImplementedException();
        }

        [DebuggerStepThrough]
        public static Exception ObjectDisposed(string objectName)
        {
            return new ObjectDisposedException(objectName);
        }

        [DebuggerStepThrough]
        public static Exception ObjectDisposed(string objectName, string message, params object[] args)
        {
            return new ObjectDisposedException(objectName, string.Format(CultureInfo.CurrentCulture, message, args));
        }

        [DebuggerStepThrough]
        public static Exception NoElements()
        {
            return new InvalidOperationException("Sequence contains no elements.");
        }

        [DebuggerStepThrough]
        public static Exception MoreThanOneElement()
        {
            return new InvalidOperationException("Sequence contains more than one element.");
        }
    }
}