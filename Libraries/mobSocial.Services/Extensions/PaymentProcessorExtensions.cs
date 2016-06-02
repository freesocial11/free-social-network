using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Exception;
using mobSocial.Core.Plugins.Extensibles.Payments;
using mobSocial.Data.Entity.Payments;

namespace mobSocial.Services.Extensions
{
    public static class PaymentProcessorExtensions
    {
        public static bool Supports(this IPaymentProcessorPlugin paymentProcessorPlugin, PaymentMethodType methodType)
        {
            return paymentProcessorPlugin.SupportedMethodTypes.Contains(methodType);
        }

        public static T GetParameterAs<T>(this ITransactionRequestBase request, string parameterName)
        {
            if (request == null)
                throw new mobSocialException("Can't read a null request");

            if (!request.Parameters.ContainsKey(parameterName))
                return default(T);

            return (T) request.Parameters[parameterName];
        }

        public static T GetParameterAs<T>(this ITransactionResultBase result, string parameterName)
        {
            if(result == null)
                throw new mobSocialException("Can't read a null result");

            if (!result.ResponseParameters.ContainsKey(parameterName))
                return default(T);
           
            return (T)result.ResponseParameters[parameterName];
        }

        /// <summary>
        /// Sets a parameter in transaction results
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public static void SetParameter(this ITransactionResultBase result, string parameterName, object parameterValue)
        {
            if (result.ResponseParameters.ContainsKey(parameterName))
                result.ResponseParameters[parameterName] = parameterValue;
            else
            {
                result.ResponseParameters.Add(parameterName, parameterValue);
            }
        }

        /// <summary>
        /// Sets a response code in a payment transaction
        /// </summary>
        /// <param name="paymentTransaction"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public static void SetTransactionCode(this PaymentTransaction paymentTransaction, string parameterName,
            object parameterValue)
        {
            if(paymentTransaction == null)
                throw new mobSocialException("Can't read null payment transaction");

            if (paymentTransaction.TransactionCodes.ContainsKey(parameterName))
                paymentTransaction.TransactionCodes[parameterName] = parameterValue;
            else
                paymentTransaction.TransactionCodes.Add(parameterName, parameterValue);
        }

        /// <summary>
        /// Sets response transaction codes in a payment transaction
        /// </summary>
        /// <param name="paymentTransaction"></param>
        /// <param name="transactionCodes"></param>
        public static void SetTransactionCodes(this PaymentTransaction paymentTransaction,
            Dictionary<string, object> transactionCodes)
        {
            foreach (var code in transactionCodes)
            {
                paymentTransaction.SetTransactionCode(code.Key, code.Value);
            }
        }
        /// <summary>
        /// Gets the value of named response code from payment transaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paymentTransaction"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static T GetTransactionCodeAs<T>(this PaymentTransaction paymentTransaction, string parameterName)
        {
            if (paymentTransaction == null)
                throw new mobSocialException("Can't read a null result");

            if (!paymentTransaction.TransactionCodes.ContainsKey(parameterName))
                return default(T);

            return (T)paymentTransaction.TransactionCodes[parameterName];
        }
    }
}