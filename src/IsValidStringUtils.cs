using System;
using System.Text.RegularExpressions;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils
{
    public static class IsValidStringUtils
    {
        #region Email

        public const int EmailMinLength = 5;
        public const int EmailMaxLength = 64;

        private const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

        /// <summary>
        /// Is Valid Email Address
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="exception"></param>
        /// <exception cref="ArgumentNullException">Is null or empty or white space</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid length</exception>
        /// <exception cref="ArgumentException">Invalid pattern</exception>
        public static bool IsValidEmailAddress(string emailAddress, out Exception exception)
        {
            try
            {
                emailAddress.ThrowIfNullOrWhiteSpace(nameof(emailAddress));
                emailAddress.Length.ThrowIfNotWithinInclusive(EmailMinLength, EmailMaxLength, nameof(emailAddress));
                emailAddress.ThrowIfNotMatchRegEx(MatchEmailPattern, nameof(emailAddress));

                exception = null;
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                exception = e;
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public static bool IsValidEmailAdress(string emailAdress)
        {
            return !string.IsNullOrWhiteSpace(emailAdress) && Regex.IsMatch(emailAdress, MatchEmailPattern);
        }
        #endregion

        #region UserName

        public const int UserNameMinLength = 3;
        public const int UserNameMaxLength = 15;

        private const string MatchUserNamePattern =
            "^[A-Za-z0-9]{3,15}$";

        private const string MatchUserNameClanPattern =
            "^[A-Za-z0-9_]{3,15}$";

        /// <summary>
        /// Is Valid User Name
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="exception"></param>
        /// <param name="allowClanTag"></param>
        /// <exception cref="ArgumentNullException">Is null or empty or white space</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid length</exception>
        /// <exception cref="ArgumentException">Invalid pattern</exception>
        public static bool IsValidUserName(string userName, out Exception exception,
            bool allowClanTag = false)
        {
            try
            {
                userName.ThrowIfNullOrWhiteSpace(nameof(userName));
                userName.Length.ThrowIfNotWithinInclusive(UserNameMinLength, UserNameMaxLength, nameof(userName));
                userName.ThrowIfNotMatchRegEx(allowClanTag ? MatchUserNameClanPattern : MatchUserNamePattern,
                    nameof(userName));

                exception = null;
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                exception = e;
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public static bool IsValidUserName(string userName)
        {
            return !string.IsNullOrWhiteSpace(userName) && Regex.IsMatch(userName, MatchUserNamePattern);
        }

        #endregion

        #region Password

        public const int PasswordMinLength = 3;
        public const int PasswordMaxLength = 15;

        /// <summary>
        /// Is Valid Password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="exception"></param>
        /// <exception cref="ArgumentNullException">Is null or empty or white space</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid length</exception>
        /// <exception cref="ArgumentException">Invalid pattern</exception>
        public static bool IsValidPassword(string password, out Exception exception)
        {
            try
            {
                password.ThrowIfNullOrWhiteSpace(nameof(password));
                password.Length.ThrowIfNotWithinInclusive(PasswordMinLength, PasswordMaxLength, nameof(password));
                if (password.Contains("'") || password.Contains("\""))
                    throw new ArgumentException(string.Empty, nameof(password));
                exception = null;
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                exception = e;
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password);
        }

        #endregion

        #region VerifyKey

        public const int VerifyKeyLength = 32;

        /// <summary>
        /// Is Valid Password
        /// </summary>
        /// <param name="verifyKey"></param>
        /// <param name="exception"></param>
        /// <exception cref="ArgumentNullException">Is null or empty or white space</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid length</exception>
        /// <exception cref="ArgumentException">Invalid pattern</exception>
        public static bool IsValidVerifyKey(string verifyKey, out Exception exception)
        {
            try
            {
                verifyKey.ThrowIfNullOrWhiteSpace(nameof(verifyKey));
                verifyKey.Length.ThrowIfNotEquals(VerifyKeyLength, nameof(verifyKey));
                exception = null;
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                exception = e;
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        #endregion
    }
}