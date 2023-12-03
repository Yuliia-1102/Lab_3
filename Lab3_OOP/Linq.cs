using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3_OOP
{
    internal class Linq
    {
        private bool IsValidPickerValue(string workValue, string criteriaValue)
        {
            return string.IsNullOrEmpty(criteriaValue) || workValue.Equals(criteriaValue);
        }

        public void Search(SearchCriteria criteria, ObservableCollection<ScienceWorker> data, ObservableCollection<ScienceWorker> results)
        {
            var workers = (from worker in data
                        where(
                         IsValidPickerValue(worker.AuthorName, criteria.AuthorName) &&
                         IsValidPickerValue(worker.Faculty, criteria.Faculty) &&
                         IsValidPickerValue(worker.Department, criteria.Department) 
                        )
                        select worker).ToList();

            results.Clear();
            foreach (ScienceWorker worker in workers)
            {
                results.Add(worker);
            }
        }
    }
}
