using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Structura.Shared.MessageBus
{   
    internal static class Check
    {
        /// <summary>
        /// Precondition check - should run regardless of preprocessor directives.
        /// </summary>
        public static void Require(bool assertion, string messageFormatString, params object[] formatStringParameters)
        {
            Require<PreconditionException>(assertion, messageFormatString, formatStringParameters);
        }

        /// <summary>
        /// Precondition check - should run regardless of preprocessor directives. Throws exception of type T. T must contain
        /// a default constructor and a constructor that accepts a string argument for the message.
        /// </summary>
        public static void Require<T>(bool assertion, string messageFormatString, params object[] formatStringParameters) where T : Exception, new()
        {
            PerformCheck(assertion,
                () => ThrowExceptionOfType<T>(messageFormatString, formatStringParameters),
                CheckType.Precondition,
                messageFormatString,
                formatStringParameters);
        }

        private static string GetStackFrameString()
        {
            var frames = new StackTrace().GetFrames();
            var method = "?";
            var declaringType = "?";
            // Frames: 
            //     0: Current method (GetStackFrameString)
            //     1: Caller
            //     2: Caller of Caller 
            //     etc
            for (var frameNum = 5; frameNum < frames.Count() && frameNum < 15; frameNum++)
            {
                var theMethod = frames[frameNum].GetMethod();
                if (theMethod.DeclaringType != null && theMethod.DeclaringType.Name != "Check" && theMethod.DeclaringType.Name != "UpdateDelegates")
                {
                    method = theMethod.Name;
                    declaringType = theMethod.DeclaringType.FullName;
                    break;
                }
            }
            return string.Format("{0}.{1}.", declaringType, method);
        }

        /// <summary>
        /// Set this if you wish to use Trace Assert statements 
        /// instead of exception handling. 
        /// (The Check class uses exception handling by default.)
        /// </summary>
        public static bool UseAssertions
        {
            get
            {
                return _useAssertions;
            }
            set
            {
                _useAssertions = value;
            }
        }

        private static string FormattedString(string messageFormatString, params object[] formatStringParameters)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}. At method {1}",
                                        formatStringParameters.Length > 0
                                    ? String.Format(messageFormatString, formatStringParameters)
                                    : messageFormatString
                               , GetStackFrameString());
        }

        private static void PerformCheck(bool assertion, Action exceptionImpl, CheckType checkType,
            string messageFormatString, params object[] formatStringParameters)
        {
            if (UseExceptions)
            {
                if (!assertion) exceptionImpl();
            }
            else if (!assertion)
                Trace.Assert(assertion, Enum.GetName(typeof(CheckType), checkType) +
                    " failed: " + FormattedString(messageFormatString, formatStringParameters));
        }

        private static void ThrowExceptionOfType<T>(string messageFormatString,
                params object[] formatStringParameters) where T : Exception, new()
        {
            var message = FormattedString(messageFormatString, formatStringParameters);
            var classType = typeof(T);
            var classConstructor = classType.GetConstructor(new[] { message.GetType() });
            if (null == classConstructor)
            {
                throw new Exception(
                    string.Format(
                        "Cannot instantiate an exception of type '{0}' because no appropriate constructor is found. " +
                        "Add a constructor with a string argument to your exception class."
                            , classType.Name));
            }
            T classInstance = (T)classConstructor.Invoke(new object[] { message });
            throw classInstance;
        }


        /// <summary>
        /// Is exception handling being used?
        /// </summary>
        private static bool UseExceptions
        {
            get
            {
                return !_useAssertions;
            }
        }

        // Are trace assertion statements being used? 
        // Default is to use exception handling.
        private static bool _useAssertions;

        private enum CheckType
        {
            Precondition,
        }
    } // End Check

    /// <summary>
    /// Exception raised when a contract is broken.
    /// Catch this exception type if you wish to differentiate between 
    /// any DesignByContract exception and other runtime exceptions.
    ///  
    /// </summary>
    public class DesignByContractException : ApplicationException
    {
        protected DesignByContractException() { }
        protected DesignByContractException(string message) : base(message) { }
        protected DesignByContractException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Exception raised when a precondition fails.
    /// </summary>
    public class PreconditionException : DesignByContractException
    {
        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException() { }
        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException(string message) : base(message) { }
        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException(string message, Exception inner) : base(message, inner) { }
    }
}