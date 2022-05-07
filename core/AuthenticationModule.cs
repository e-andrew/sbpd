using System;
using System.Threading;
using Cerberus.controller;

namespace Cerberus.core
{
    public class AuthenticationModule
    {
        private Buffer _buffer;
        private Config _authenticationConfig;
        private Random _random = new Random();
        private const int RequestInterval = 60000;
        private const int ErrorQuantity = 3;
        private const int MinAuthenticationNumber = 100;
        private const int MaxAuthenticationNumber = 1000;
        private const string RequestText = "REQUEST";
        private const string SuccessText = "SUCCESS";
        private const string FailureText = "FAILURE";
        private const string DateFormat = "HH:mm:ss dd.MM.yyyy";
        private int _questionQuantity = 4;
        private int _result;
        private bool _isNotPaused;
        private bool _isStopped;
        public AuthenticationModule(Storage storage, ControlWindow controlWindow)
        {
            _authenticationConfig = storage.GetConfig("authentication");
            _isNotPaused = false;
            _isStopped = false;
            Thread authenticationProcedure = new Thread(Authenticate);
            authenticationProcedure.Start(controlWindow);
        }
        public void Enter(Buffer buffer)
        {
            _buffer = buffer;
            _questionQuantity = 4;
            _isNotPaused = true;
            _result = 0;
        }
        
        public void Leave()
        {
            _isNotPaused = false;
        }
        
        public void Disconnect()
        {
            _isStopped = true;
        }
        private void Authenticate(object window)
        { 
            ControlWindow controlWindow = (ControlWindow) window;
            while (true)
            {
                Thread.Sleep(RequestInterval);
                if (_isStopped) break;
                if (_isNotPaused && _questionQuantity > 0)
                {
                    controlWindow.Dispatcher.Invoke((Action)(() =>
                    {
                        controlWindow.OpenAuthenticationWindow();
                    }));
                }
            }
        }
        public bool IsNotAuthenticated()
        {
            return _result == -1;
        }
        
        public bool IsAuthenticated()
        {
            return _result == 0;
        }

        public bool IsNeedLeave()
        {
            return _result == 1;
        }
        
        public bool IsNeedRegister()
        {
            return _result == 2;
        }
        private bool IsNotManyErrors()
        {
            return _authenticationConfig.Count(new[] {FailureText, _buffer.GetLogin()},
                new[] {0, 1}) < ErrorQuantity;
        }
        public void AuthenticationRequest()
        {
            _questionQuantity -= 1;
            _result = -1;
            int question = _random.Next(MinAuthenticationNumber, MaxAuthenticationNumber);
            int answer = (int) Math.Floor(Math.Sqrt(_buffer.GetCode() + question));
            
            _buffer.Verify(question, answer);

            AppendRecord(RequestText, _buffer.GetLogin(), _buffer.GetCode().ToString(),
                question.ToString(), answer.ToString(), DateTime.Now.ToString(DateFormat));
        }
        private int ParseAnswer(string givenAnswer)
        {
            int answer = 0;
            if (int.TryParse(givenAnswer, out var innerAnswer))
            {
                answer = innerAnswer;
            }
            return answer;
        }
        public void AuthenticationResponse(string givenAnswer)
        {
            bool decision = _buffer.GetAnswer() == ParseAnswer(givenAnswer);
            _result = decision ? 0 : IsNotManyErrors() ? 1 : 2;
            
            AppendRecord(decision ? SuccessText : FailureText, _buffer.GetLogin(), 
                _buffer.GetCode().ToString(), _buffer.GetQuestion().ToString(),
                givenAnswer, DateTime.Now.ToString(DateFormat));
        }
        private void AppendRecord(string type, string login, string code, string question, string answer, string time)
        {
            _authenticationConfig.Insert(new []{type, login, code, question, answer, time});
        }
    }
}