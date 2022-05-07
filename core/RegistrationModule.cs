using System;

namespace Cerberus.core
{
    public class RegistrationModule
    {
        private readonly Config _registrationConfig;
        private readonly Random _random = new Random();
        private const int MinPasswordLength = 8;
        private const int MaxPasswordLength = 20;
        private const int MaxUserSize = 8;
        private const int MinAuthenticationNumber = 100;
        private const int MaxAuthenticationNumber = 1000;
        private const int PasswordDuration = 4;
        private const int SaltSize = 20;
        private const string DateFormat = "yyyy.MM.dd.HH.mm.ss";
        private const string YesText = "yes";
        private const string NoText = "no";
        public RegistrationModule(Storage storage)
        {
            _registrationConfig = storage.GetConfig("registration");
        }
        private bool CheckUserLimit()
        {
            return _registrationConfig.GetSize() < MaxUserSize;
        }
        private bool CheckLogin(string login)
        {
            bool decision = login.Length > 0;
            
            foreach (char symbol in login)
            {
                if (!(symbol == 46 || symbol > 47 && symbol < 58 || symbol > 63 && symbol < 91 ||
                    symbol > 96 && symbol < 123))
                {
                    decision = false;
                    break;
                }
            }
            
            return decision;
        }
        private bool CheckUserNotExists(string login)
        {
            string[] emptyRecord = new string[0];
            return _registrationConfig.FindRecord(login, 0, emptyRecord) == emptyRecord;
        }
        private bool CheckPassword(string password, string repeat)
        {
            bool lengthIsGood = MinPasswordLength <= password.Length && password.Length <= MaxPasswordLength;
            bool repeatIsGood = password == repeat;
            bool symbolsIsGood = true;
            bool hasDigitSymbol = false;
            bool hasSpecialSymbol = false;
            bool hasUpperSymbol = false;
            bool hasLowerSymbol = false;
            
            foreach (char symbol in password)
            {
                if (!(symbol > 47 && symbol < 58 || symbol > 63 && symbol < 91 ||
                      symbol > 96 && symbol < 123))
                {
                    symbolsIsGood = false;
                }

                if (symbol > 47 && symbol < 58)
                {
                    hasDigitSymbol = true;
                }
                else if (symbol == 64) {
                    hasSpecialSymbol = true;
                }
                else if (symbol > 64 && symbol < 91)
                {
                    hasUpperSymbol = true;
                }
                else if (symbol > 96 && symbol < 123)
                {
                    hasLowerSymbol = true;
                }
            }
            
            return lengthIsGood && repeatIsGood && symbolsIsGood && hasUpperSymbol && hasLowerSymbol && hasDigitSymbol && hasSpecialSymbol;
        }
        private bool CheckAdmin()
        {
            return _registrationConfig.GetSize() == 0;
        }
        public bool Register(string login, string password, string repeat)
        {
            bool decision = CheckUserLimit() && CheckLogin(login) && CheckUserNotExists(login) && CheckPassword(password, repeat);
                
            if (decision)
            {
                DateTime creation = DateTime.Now;
                bool isAdmin = CheckAdmin();
                DateTime expiration = isAdmin ? DateTime.MaxValue : creation.AddDays(PasswordDuration);
                
                string[] userRecord = new string[8];
                userRecord[0] = login;
                userRecord[1] = CryptographyModule.GenerateSalt(SaltSize);
                userRecord[2] = CryptographyModule.HashPassword(password, userRecord[1]);
                userRecord[3] = creation.ToString(DateFormat);
                userRecord[4] = expiration.ToString(DateFormat);
                userRecord[5] = isAdmin ? YesText : NoText;
                userRecord[6] = isAdmin ? "admin" : "user";
                userRecord[7] = _random.Next(MinAuthenticationNumber, MaxAuthenticationNumber).ToString();
                
                _registrationConfig.Insert(userRecord);
            }

            return decision;
        }
        private bool CheckHashPassword(string password, string salt, string hash)
        {
            return CryptographyModule.HashPassword(password, salt).Equals(hash);
        }
        private bool CheckTimeLimit(string creation, string expiration)
        {
            DateTime today = DateTime.Now;
            string[] ct = creation.Split('.');
            string[] et = expiration.Split('.');
            DateTime creationTime = new DateTime(int.Parse(ct[0]), int.Parse(ct[1]), int.Parse(ct[2]),
                int.Parse(ct[3]), int.Parse(ct[4]), int.Parse(ct[5]));
            DateTime expirationTime = new DateTime(int.Parse(et[0]), int.Parse(et[1]), int.Parse(et[2]),
                int.Parse(et[3]), int.Parse(et[4]), int.Parse(et[5]));
            return today >= creationTime && today <= expirationTime;
        }
        private bool CheckAcceptance(string acceptance)
        {
            return acceptance.Equals(YesText);
        }
        public void Enter(Buffer buffer, string login, string password)
        {
            string[] user = _registrationConfig.FindRecord(login, 0, new string[8]);

            if (user.Length < 8) { user = new string[8]; }
            
            if (CheckHashPassword(password, user[1], user[2]) &&
                CheckTimeLimit(user[3], user[4]) && CheckAcceptance(user[5]))
            {
                buffer.Enter(user[0], user[6], int.Parse(user[7]));
            }
        }
        public void Unregister(Buffer buffer)
        {
            if (!buffer.IsAdmin())
            {
                _registrationConfig.Exclude(buffer.GetLogin(), 0);
            }
        }
    }
}