using System;
using Delta.SmartCard.Logging;
using PCSC;

namespace Delta.SmartCard
{
    internal static class SmartCardHelper
    {
        private static ILogService log = LogManager.GetLogger(typeof(SmartCardHelper));

        public static bool EnsureOK(this SCardContext context, bool throwException = true)
        {
            if (context.IsValid())
                return true;

            return context.CheckValidity().EnsureOK(throwException, "Unable to establish a valid PC/SC context");
        }

        public static bool EnsureOK(this SCardError sc, bool throwException = true, string message = null)
        {
            if (sc == SCardError.Success)
                return true;

            var errorMessage = string.IsNullOrEmpty(message) ?
                string.Format("Invalid PC/SC Operation: {0}", sc) :
                string.Format("{0}: {1}", message, sc);

            if (throwException)
                throw new InvalidOperationException(errorMessage);
            else log.Error(errorMessage);

            return false;
        }
    }
}
