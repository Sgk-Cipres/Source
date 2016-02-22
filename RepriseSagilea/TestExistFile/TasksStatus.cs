using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestExistFile
{
    public class TasksStatus
    {
        private Dictionary<string, Status> _tasklist;
        public Dictionary<string, Status> TasksList { get { return _tasklist; } }

        public string CurrentTask { get; set; }
        public string CurrentTaskDuration { get; set; }
        public string CurrentTaskMessage { get; set; }

        public TasksStatus()
        {
            Init();
        }

        public Status GetTaskStatus(string task)
        {
            if (_tasklist.ContainsKey(task))
                return _tasklist[task];
            else
                return Status.NO;
        }

        public void SetTaskStatus(string task, Status status = Status.SB)
        {
            if (_tasklist.ContainsKey(task))
                _tasklist[task] = status;
            else
                _tasklist.Add(task, status);

            CurrentTask = task;
        }

        public void Init()
        {
            _tasklist = new Dictionary<string, Status>();
        }

        public void ResetList(List<string> tasklist)
        {
            Init();

            if (_tasklist == null)
                _tasklist = new Dictionary<string, Status>();

            foreach (string t in tasklist)
            {
                _tasklist.Add(t, Status.SB);
            }

            CurrentTask = string.Empty;
            CurrentTaskDuration = string.Empty;
            CurrentTaskMessage = string.Empty;
        }
    }

    /// <summary>
    /// statut de la tâche
    /// KO = en erreur
    /// OK = reussie
    /// SB = en attente (stand by)
    /// NO = n'existe pas
    /// </summary>
    public enum Status
    {
        KO,OK,SB,NO
    }
}
