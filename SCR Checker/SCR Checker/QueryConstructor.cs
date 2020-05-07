using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR_Checker
{
    /// <summary>
    /// Object to build and represent a string to be used for a query, eventually using the getString() method.
    /// </summary>
    public class QueryConstructor
    {

        private const string DISPENSED = @"
SELECT V.DrugDescription, V.Quantity, Pa.Description, Pa.UnitsPerPack
FROM PMR.PrescriptionCollectionSummaryView V
JOIN PMR.PrescriptionItemDispensed P ON P.PrescriptionItemId = V.PrescriptionItemId
JOIN PackSearchView Pa ON P.PackCodeId = Pa.PackCodeId";

        // NHS number first
        // Patient first name second
        // Patient second name third
        // Patient notes fourth
        private const string PATIENTS_WITH_NOTES_DISPENSED_TO = @"
SELECT P.NhsNumber, P.GivenName, P.Surname, N.Note 
FROM PMR.PrescriptionCollectionSummaryView V 
JOIN dbo.PatientLiteView P ON V.PatientId = P.PatientId 
JOIN dbo.PatientNote N ON V.PatientId = N.PatientId ";

        private const string FILTER = " WHERE ";

        private const string FILTER_DATE = "CONVERT(VARCHAR(10), V.AddedDate, 111) = ";

        private const string DATE_FORMAT = "yyyy/MM/dd";

        private const string FILTER_AND = " AND ";

        private QueryType type;

        private List<KeyValuePair<Condition, string>> conditions = new List<KeyValuePair<Condition, string>>();

        private string sort;

        public enum QueryType
        {
            /// <summary>
            /// Grabs all items dispensed, selecting DrugDescription, Quantity, Prescription, Description, and UnitsPerPack
            /// </summary>
            DISPENSED,
            PATIENTS_WITH_NOTES_DISPENSED_TO
        }

        private enum Condition
        {
            /// <summary>
            /// Filters results by a specific day
            /// </summary>
            DATE
        }

        /// <summary>Creates an object to represent a string used for a query.</summary>
        /// <param name="type"> the type of query to run</param>
        public QueryConstructor(QueryType type)
        {
            this.type = type;
        }

        /// <summary>
        /// Filters the query to the specified day
        /// </summary>
        /// <param name="day"> the day to filter by</param>
        public void SpecificDay(DateTime day)
        {
            string dateString = day.ToString(DATE_FORMAT);
            RemoveCondition(Condition.DATE);
            AddCondition(Condition.DATE, dateString);
        }

        /// <summary>
        /// Removes a condition from the query if there is one, otherwise does nothing
        /// </summary>
        /// <param name="cond">the type of condition to remove</param>
        private void RemoveCondition(Condition cond)
        {
            if (conditions.Exists(x => x.Key == cond))
            {
                var condition = conditions.Find(x => x.Key == cond);
                conditions.Remove(condition);
            }
        }

        // Temporary hardcode
        public void SortBy()
        {
            sort = "ORDER BY P.Surname";
        }

        /// <summary>
        /// Adds a condition to the list of conditions
        /// </summary>
        /// <param name="cond">the type of condition to add</param>
        /// <param name="data">how the condition is filtered</param>
        private void AddCondition(Condition cond, string data)
        {
            conditions.Add(new KeyValuePair<Condition, string>(cond, data));
        }

        private string ConditionsStringBuilder(List<KeyValuePair<Condition, string>> conditionList)
        {
            string str = "";
            if (conditionList.Count > 0)
            {
                KeyValuePair<Condition, string> condition = conditionList.First();
                switch (condition.Key)
                {
                    case Condition.DATE:
                        str = FILTER_DATE + "'" + condition.Value + "'";
                        break;
                }
                conditionList.RemoveAt(0);
                if (conditionList.Count > 0)
                {
                    str = str + FILTER_AND + ConditionsStringBuilder(conditionList);
                }
            }

            return str;
        }

        /// <summary>
        /// Used after specifying all conditions to generate a string to use for the query.
        /// </summary>
        /// <returns>string to be used when making an SQL query.</returns>
        public override string ToString()
        {
            string str = "";
            switch (type)
            {
                case QueryType.DISPENSED:
                    str = DISPENSED;
                    break;

                case QueryType.PATIENTS_WITH_NOTES_DISPENSED_TO:
                    str = PATIENTS_WITH_NOTES_DISPENSED_TO;
                    break;
            }

            if (conditions.Count > 0)
            {
                str = str + FILTER + ConditionsStringBuilder(conditions);
            }

            if(sort != null)
            {
                str = str + sort;
            }

            return str;
        }
    }
}
