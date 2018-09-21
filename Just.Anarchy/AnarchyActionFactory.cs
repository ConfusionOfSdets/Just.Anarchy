using System;
using System.Text.RegularExpressions;

namespace Just.Anarchy
{
    public class AnarchyActionFactory : IAnarchyActionFactory
    {
        public ICauseAnarchy AnarchyAction { get; }
        public bool IsActive { get; private set; }
        public string TargetPattern => _matchTargetPattern.ToString();
        public Schedule ExecutionSchedule { get; private set; }

        private Regex _matchTargetPattern;

        public AnarchyActionFactory(ICauseAnarchy anarchyAction)
        {
            AnarchyAction = anarchyAction;
            IsActive = false;
        }

        public void HandleRequest(string requestUrl)
        {
            if (ShouldHandleRequest(requestUrl))
            {
                AnarchyAction.ExecuteAsync();
            }
        }

        public void Start()
        {
            IsActive = true;
        }

        public void Stop()
        {
            IsActive = false;
        }

        public void WithSchedule(Schedule schedule)
        {
            this.ExecutionSchedule = schedule;           
        }

        public void ForTargetPattern(string pattern)
        {
            if (pattern == null)
            {
                _matchTargetPattern = null;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(pattern))
                {
                    throw new ArgumentException("The target pattern needs to be a valid .net regular expression");
                }
                _matchTargetPattern = new Regex(pattern, RegexOptions.Compiled & RegexOptions.Singleline, TimeSpan.FromSeconds(1));
            }    
        }

        private bool ShouldHandleRequest(string requestUrl) => 
            _matchTargetPattern != null && 
            _matchTargetPattern.IsMatch(requestUrl);
        
    }
}
